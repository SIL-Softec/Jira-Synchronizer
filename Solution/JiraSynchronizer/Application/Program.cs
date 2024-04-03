﻿using JiraSynchronizer.Application.Controllers;
using JiraSynchronizer.Application.Services;
using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Enums;
using JiraSynchronizer.Core.Interfaces;
using JiraSynchronizer.Core.Services;
using JiraSynchronizer.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Configuration;

namespace JiraSynchronizer.Application;

public class Program
{
    // ConnectionString ist in App.config als "default" definiert
    private static string defaultConnection;
    // Eine SQL Connection wird für alle Services erstellt
    private static SqlConnection sqlConnection;
    static async Task Main(string[] args)
    {
        // Initialize Services
        AuthorizationService authService = new AuthorizationService();
        LoggingService logService = new LoggingService();
        ViewModelService viewModelService = new ViewModelService();

        // Mutex stellt sicher dass nur eine Instanz der Applikation auf einmal läuft
        string mutex_id = "Jira Synchronizer";
        using (Mutex mutex = new Mutex(false, mutex_id))
        {
            if (!mutex.WaitOne(0, false))
            {
                logService.Log(LogCategory.Error, "Eine Instanz der Applikation läuft bereits");
                logService.Log(LogCategory.ApplicationAborted, "Applikation wurde abgebrochen\n");
                return;
            }
            // Initialise Log 
            logService.Log(LogCategory.LogfileInitialized, "Log Datei wurde initialisiert\n");
            logService.Log(LogCategory.ApplicationStarted, "Applikation wurde gestartet\n");

            // Configure SQL Connection using App.config
            if (System.Configuration.ConfigurationManager.ConnectionStrings["Default"] != null)
            {
                defaultConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                sqlConnection = new SqlConnection(defaultConnection);
            } else
            {
                logService.Log(LogCategory.Error, "Connection String ist undefiniert oder nicht gültig");
                logService.Log(LogCategory.ApplicationAborted, "Applikation wurde abgebrochen");
                return;
            }

            // Configure appsettings
            IConfigurationRoot config;
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false);
                config = builder.Build();
            } catch
            {
                logService.Log(LogCategory.Error, "Zwingend notwendige Datei \"appsettings.json\" konnte nicht gefunden werden");
                logService.Log(LogCategory.ApplicationAborted, "Applikation wurde abgebrochen");
                return;
            }

            // Set development user if dev mode is enabled
            bool isDevelopment = false;
            int? devUser = null;
            if (args.Length > 0 && (args.Any(a => a == "-dev") || args.Any(a => a == "-d")))
            {
                isDevelopment = true;
                devUser = Int32.Parse(config.GetSection("DevUser").Value);
                logService.Log(LogCategory.Information, $"Applikation läuft in Development Modus, Daten werden auf den Benutzer mit der Id \"{devUser}\" gespeichert\n");
            }
            if (args.Length > 0 && args.Any(a => a != "-dev" && a != "-d"))
            {
                logService.Log(LogCategory.Warning, "Applikation wurde mit ungültigen Argumenten aufgerufen\n");
            }

            // Initialize Jira Controllers
            IssueController issueController = new IssueController();
            WorklogController worklogController = new WorklogController();
            // Initialize Database Controllers
            try
            {
                sqlConnection.Open();

            } catch
            {

                logService.Log(LogCategory.Error, "Connection String ist undefiniert oder nicht gültig");
                logService.Log(LogCategory.ApplicationAborted, "Applikation wurde abgebrochen");
                return;
            }
            LeistungserfassungController leistungserfassungController = new LeistungserfassungController(sqlConnection);
            WhitelistController whitelistController = new WhitelistController(sqlConnection);
            UserController userController = new UserController(sqlConnection);
            ProjektMitarbeiterController projektMitarbeiterController = new ProjektMitarbeiterController(sqlConnection);
            ProjektController projektController = new ProjektController(sqlConnection);

            // Whitelist wird von der Datenbank importiert und in JiraProjectViewModel übersetzt
            List<JiraProjectViewModel> jiraProjects = viewModelService.GenerateJiraProjectViewModels(whitelistController.GetAllWhitelists());

            // Alle issue keys der jeweiligen Projekte werden von der Jira REST API importiert
            foreach (JiraProjectViewModel project in jiraProjects)
            {
                try
                {
                    project.Issues = await issueController.GetIssuesAsync(project.ProjectName);
                } catch
                {
                    logService.Log(LogCategory.Error, "API-Token für die Jira API ist undefiniert oder ungültig");
                    logService.Log(LogCategory.ApplicationAborted, "Applikation wurde abgebrochen");
                    return;
                }
            }
            logService.Log(LogCategory.Success, "Issues wurden erfolgreich geladen\n");

            // Alle worklogs, jünger als sieben Tage, der jeweiligen Issues werden von der Jira REST API importiert
            foreach (JiraProjectViewModel project in jiraProjects)
            {
                foreach (IssueViewModel issue in project.Issues)
                {
                    issue.Worklogs = await worklogController.GetWorklogsAsync(issue.IssueName);
                }
            }
            logService.Log(LogCategory.Success, "Worklogs wurden erfolgreich geladen\n");

            // User und die Projekte auf welchen sie Berechtigungen haben werden importiert
            List<UserViewModel> userList = userController.GetAllUsers();
            List<ProjektMitarbeiterViewModel> projektMitarbeiterList = projektMitarbeiterController.GetAllProjektMitarbeiters();

            // Leistungen welche jünger als sieben Tage sind werden von der LEIS Datenbank importiert und in ein minimales ViewModel geschrieben welches nur die wichtigsten Daten enthält
            List<LeistungserfassungViewModel> minimaleLeistungserfassungen = leistungserfassungController.GetAllLeistungserfassungen();

            // Bei allen worklogs wird geschrieben ob die User authorisiert sind und ob das worklog in der Datenbank schon vorhanden ist
            List<WorklogViewModel> worklogs = new List<WorklogViewModel>();
            foreach (JiraProjectViewModel project in jiraProjects)
            {
                foreach (IssueViewModel issue in project.Issues)
                {
                    foreach (WorklogViewModel worklog in issue.Worklogs)
                    {
                        worklog.IsAuthorized = authService.IsAuthorized(userList, projektMitarbeiterList, worklog.Email, project.LeisProjectId);
                        if (isDevelopment) worklog.IsAuthorized = true;
                        worklog.ExistsOnDatabase = minimaleLeistungserfassungen.Any(ml => ml.JiraProjectId == worklog.JiraBuchungId);
                        worklog.JiraProjekt = project.ProjectName;

                        // Alle logs die authorisierte Nutzer haben und in der Datenbank noch nicht existieren werden in eine übersichtlichere Liste übertragen
                        if (worklog.IsAuthorized && !worklog.ExistsOnDatabase || isDevelopment) worklogs.Add(worklog);

                        // Enthält ein worklog eine Leistung von >24h, wird eine Warnung in den Log geschrieben
                        if (((double)worklog.TimeSpentSeconds) / 3600 > 24) logService.Log(LogCategory.OvertimeWarning, $"Benutzer mit der Email \"{worklog.Email}\" hat über 24 Stunden an Issue \"{issue.IssueName}\" gearbeitet, Eintrag mit Startdatum \"{worklog.Started}\"\n");
                    }
                }
            }
            logService.Log(LogCategory.Information, "Worklogs wurden authorisiert und überprüft\n");

            // ProjektViewModels werden aus Datenbank geladen
            List<ProjektViewModel> projekte = projektController.GetAllProjekte();

            // Worklogs werden in LeistungserfassungViewModels übertragen und auf der Datenbank als Leistungserfassungen gespeichert
            List<LeistungserfassungViewModel> leistungserfassungViewModels = viewModelService.GenerateLeistungserfassungViewModels(worklogs, userList, jiraProjects, projekte, devUser, isDevelopment);
            leistungserfassungController.AddLeistungserfassungen(leistungserfassungViewModels);
            logService.Log(LogCategory.Success, "Daten wurden erfolgreich importiert");

            sqlConnection.Close();
            logService.Log(LogCategory.ApplicationStopped, "Applikation wurde beendet");
        }        
    }
}