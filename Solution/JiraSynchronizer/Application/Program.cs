using JiraSynchronizer.Application.Controllers;
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
    // ConnectionString is defined in App.config as "Default"
    private static string defaultConnection;
    // SQL Connection is readied for all DatabaseServices
    private static SqlConnection sqlConnection;
    static async Task Main(string[] args)
    {
        // Initialize Services
        AuthorizationService authService = new AuthorizationService();
        LoggingService logService = new LoggingService();
        ViewModelService viewModelService = new ViewModelService();

        // Mutex ensures that only one instance of this application ever runs at any given time
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

            // Initialize Database Controllers and ready SQL Connection
            try
            {
                sqlConnection.Open();

            } catch
            {

                logService.Log(LogCategory.Error, "Connection String ist undefiniert oder ungültig");
                logService.Log(LogCategory.ApplicationAborted, "Applikation wurde abgebrochen");
                return;
            }
            LeistungserfassungController leistungserfassungController = new LeistungserfassungController(sqlConnection);
            WhitelistController whitelistController = new WhitelistController(sqlConnection);
            UserController userController = new UserController(sqlConnection);
            ProjektMitarbeiterController projektMitarbeiterController = new ProjektMitarbeiterController(sqlConnection);
            ProjektController projektController = new ProjektController(sqlConnection);

            // Whitelist is imported from Database and translated into a JiraProjectViewModel
            List<JiraProjectViewModel> jiraProjects = viewModelService.GenerateJiraProjectViewModels(whitelistController.GetAllWhitelists());

            // All issue keys of projects present in "jiraProjects" are imported from Jira API
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

            // All worklogs younger than seven days are imported from Jira API 
            foreach (JiraProjectViewModel project in jiraProjects)
            {
                foreach (IssueViewModel issue in project.Issues)
                {
                    issue.Worklogs = await worklogController.GetWorklogsAsync(issue.IssueName);
                }
            }
            logService.Log(LogCategory.Success, "Worklogs wurden erfolgreich geladen\n");

            // User and the projects they're authorized to write on are imported from Database
            List<UserViewModel> userList = userController.GetAllUsers();
            List<ProjektMitarbeiterViewModel> projektMitarbeiterList = projektMitarbeiterController.GetAllProjektMitarbeiters();

            // "Leistungen" younger than seven days are imported from LEIS DB and translated into ViewModel which contains only the most important data
            List<LeistungserfassungViewModel> minimaleLeistungserfassungen = leistungserfassungController.GetAllLeistungserfassungen();

            // For each worklog authorization of their user is checked and it's checked if the worklog has already been saved to the LEIS DB in the past
            List<WorklogViewModel> worklogs = new List<WorklogViewModel>();
            foreach (JiraProjectViewModel project in jiraProjects)
            {
                foreach (IssueViewModel issue in project.Issues)
                {
                    foreach (WorklogViewModel worklog in issue.Worklogs)
                    {
                        worklog.IsAuthorized = !isDevelopment? authService.IsAuthorized(userList, projektMitarbeiterList, worklog.Email, project.LeisProjectId) : true;
                        worklog.ExistsOnDatabase = minimaleLeistungserfassungen.Any(ml => ml.JiraProjectId == worklog.JiraBuchungId);
                        worklog.JiraProjekt = project.ProjectName;
                        worklog.IssueName = issue.IssueName;

                        // All worklogs with authorized users and which do not already exist on the LEIS DB are transferred to a list
                        // If development mode is turned on, all worklogs, regardless of authorization or presence on LEIS DB are transferred to a list
                        if (worklog.IsAuthorized && !worklog.ExistsOnDatabase || isDevelopment) worklogs.Add(worklog);

                        // If a worklog has a recorded time of over 24 hours, a warning is written into the log
                        if (((double)worklog.TimeSpentSeconds) / 3600 > 24) logService.Log(LogCategory.OvertimeWarning, $"Benutzer mit der Email \"{worklog.Email}\" hat über 24 Stunden an Issue \"{issue.IssueName}\" gearbeitet, Eintrag mit Startdatum \"{worklog.Started}\"\n");
                    }
                }
            }
            if (!isDevelopment) logService.Log(LogCategory.Information, "Worklogs wurden authorisiert und überprüft\n");

            // ProjektViewModels are imported from LEIS DB
            List<ProjektViewModel> projekte = projektController.GetAllProjekte();

            // Worklogs are translated into LeistungserfassungViewModels and are saved on LEIS DB
            List<LeistungserfassungViewModel> leistungserfassungViewModels = viewModelService.GenerateLeistungserfassungViewModels(worklogs, userList, jiraProjects, projekte, devUser, isDevelopment);
            leistungserfassungController.AddLeistungserfassungen(leistungserfassungViewModels);
            logService.Log(LogCategory.Success, "Daten wurden erfolgreich importiert");

            sqlConnection.Close();
            logService.Log(LogCategory.ApplicationStopped, "Applikation wurde beendet");
        }        
    }
}