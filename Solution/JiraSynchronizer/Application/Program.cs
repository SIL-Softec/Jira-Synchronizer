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
    // ConnectionString ist in App.config als "default" definiert
    private readonly static string defaultConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
    // Eine SQL Connection wird für alle Services erstellt
    private static SqlConnection sqlConnection = new SqlConnection(defaultConnection);
    static async Task Main(string[] args)
    {
        // Configure appsettings
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);
        IConfigurationRoot config = builder.Build();
        // Set development user if dev mode is enabled
        bool isDevelopment = false;
        int? devUser = null;
        if (args.Length > 0 && (args[0] == "-dev" || args[0] == "-d"))
        {
            isDevelopment = true;
            devUser = Int32.Parse(config.GetSection("DevUser").Value);
        }

        // Initialize Jira Controllers
        IssueController issueController = new IssueController();
        WorklogController worklogController = new WorklogController();
        // Initialize Database Controllers
        sqlConnection.Open();
        LeistungserfassungController leistungserfassungController = new LeistungserfassungController(sqlConnection);
        WhitelistController whitelistController = new WhitelistController(sqlConnection);
        UserController userController = new UserController(sqlConnection);
        ProjektMitarbeiterController projektMitarbeiterController = new ProjektMitarbeiterController(sqlConnection);
        ProjektController projektController = new ProjektController(sqlConnection);

        // Initialize Services
        AuthorizationService authService = new AuthorizationService();
        LoggingService logService = new LoggingService();
        ViewModelService viewModelService = new ViewModelService();

        // Initialise Log 
        logService.Log(LogCategory.LogfileInitialized, "Logfile initialized");
        logService.Log(LogCategory.ApplicationStarted, "Application started\n");

        // Whitelist wird von der Datenbank importiert und in JiraProjectViewModel übersetzt
        List<JiraProjectViewModel> jiraProjects = viewModelService.GenerateJiraProjectViewModels(whitelistController.GetAllWhitelists());

        // Alle issue keys der jeweiligen Projekte werden von der Jira REST API importiert
        foreach (JiraProjectViewModel project in jiraProjects)
        {
            project.Issues = await issueController.GetIssuesAsync(project.ProjectName);
        }

        // Alle worklogs, jünger als sieben Tage, der jeweiligen Issues werden von der Jira REST API importiert
        foreach (JiraProjectViewModel project in jiraProjects)
        {
            foreach (IssueViewModel issue in project.Issues)
            {
                issue.Worklogs = await worklogController.GetWorklogsAsync(issue.IssueName);
            }
        }

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
                    if (worklog.IsAuthorized && !worklog.ExistsOnDatabase) worklogs.Add(worklog);

                    // Enthält ein worklog eine Leistung von >24h, wird eine Warnung in den Log geschrieben
                    if (((double)worklog.TimeSpentSeconds) / 3600 > 24) logService.Log(LogCategory.OvertimeWarning, $"User with Email {worklog.Email} worked over 24 hours on issue {issue.IssueName}, worklog starting at {worklog.Started}\n");
                }
            }
        }

        // ProjektViewModels werden aus Datenbank geladen
        List<ProjektViewModel> projekte = projektController.GetAllProjekte();

        // Worklogs werden in LeistungserfassungViewModels übertragen und auf der Datenbank als Leistungserfassungen gespeichert
        List<LeistungserfassungViewModel> leistungserfassungViewModels = viewModelService.GenerateLeistungserfassungViewModels(worklogs, userList, jiraProjects, projekte, devUser, isDevelopment);
        leistungserfassungController.AddLeistungserfassungen(leistungserfassungViewModels);
        logService.Log(LogCategory.Success, "Data was successfully imported");

        sqlConnection.Close();
        logService.Log(LogCategory.ApplicationStopped, "Application stopped");
    }
}