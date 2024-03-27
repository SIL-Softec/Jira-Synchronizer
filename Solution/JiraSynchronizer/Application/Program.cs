using JiraSynchronizer.Application.Controllers;
using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Enums;
using JiraSynchronizer.Core.Interfaces;
using JiraSynchronizer.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Configuration;

namespace JiraSynchronizer.Application;

public class Program
{
    // ConnectionString ist in App.config als "default" definiert
    private readonly static string defaultConnection = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
    // Eine SQL Connection wird für alle Services erstellt
    private static SqlConnection sqlConnection = new SqlConnection(defaultConnection);
    // Pfad zum Logfile
    private readonly static string path = @".\log.txt";
    static async Task Main(string[] args)
    {
        sqlConnection.Open();
        IssueController issueController = new IssueController();
        WorklogController worklogController = new WorklogController();
        LeistungserfassungController leistungserfassungController = new LeistungserfassungController(sqlConnection);
        WhitelistController whitelistController = new WhitelistController(sqlConnection);
        UserController userController = new UserController(sqlConnection);
        ProjektMitarbeiterController projektMitarbeiterController = new ProjektMitarbeiterController(sqlConnection);
        ProjektController projektController = new ProjektController(sqlConnection);


        // Initialise Log 
        Log(LogCategory.LogfileInitialized, "Logfile initialized");
        Log(LogCategory.ApplicationStarted, "Application started\n");

        // Whitelist wird von der Datenbank importiert und in JiraProjectViewModel übersetzt
        List<JiraProjectViewModel> jiraProjects = GenerateJiraProjects(whitelistController.GetAllWhitelists());

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

        // Bei allen worklogs wird überprüft, ob der erfasste Benutzer berechtigt ist, auf das jeweilige Projekt zu buchen
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
                    worklog.IsAuthorized = IsAuthorized(userList, projektMitarbeiterList, worklog.Email, project.LeisProjectId);
                    worklog.ExistsOnDatabase = minimaleLeistungserfassungen.Any(ml => ml.JiraProjectId == worklog.JiraBuchungId);
                    worklog.JiraProjekt = project.ProjectName;

                    // Alle logs die authorisierte Nutzer haben und in der Datenbank noch nicht existieren werden in eine übersichtlichere Liste übertragen
                    if (worklog.IsAuthorized && !worklog.ExistsOnDatabase) worklogs.Add(worklog);

                    // Enthält ein worklog eine Leistung von >24h, wird eine Warnung in den Log geschrieben
                    if (((double)worklog.TimeSpentSeconds) / 3600 > 24) Log(LogCategory.OvertimeWarning, $"User with Email {worklog.Email} worked over 24 hours on issue {issue.IssueName}, worklog starting at {worklog.Started}\n");
                }
            }
        }

        // ProjektViewModels werden aus Datenbank geladen
        List<ProjektViewModel> projekte = projektController.GetAllProjekte();

        // Worklogs werden in LeistungserfassungViewModels übertragen und auf der Datenbank als Leistungserfassungen gespeichert
        List<LeistungserfassungViewModel> leistungserfassungViewModels = GenerateLeistungserfassungViewModels(worklogs, userList, jiraProjects, projekte);
        leistungserfassungController.AddLeistungserfassungen(leistungserfassungViewModels);


        sqlConnection.Close();
        Log(LogCategory.ApplicationStopped, "Application stopped");
    }

    public static List<LeistungserfassungViewModel> GenerateLeistungserfassungViewModels(List<WorklogViewModel> worklogs, List<UserViewModel> userList, List<JiraProjectViewModel> jiraProjects, List<ProjektViewModel> projekte)
    {
        List<LeistungserfassungViewModel> leistungserfassungViewModels = new List<LeistungserfassungViewModel>();
        foreach(WorklogViewModel worklog in worklogs)
        {
            JiraProjectViewModel jiraProject = jiraProjects.FirstOrDefault(w => w.ProjectName == worklog.JiraProjekt);
            int? projektId = jiraProject != null ? jiraProject.LeisProjectId : null;
            if (projektId != null)
            {
                leistungserfassungViewModels.Add(new LeistungserfassungViewModel()
                {
                    ProjektId = projektId.Value,
                    MitarbeiterId = userList.First(u => u.UniqueName == worklog.Email).MitarbeiterId,
                    Beginn = worklog.Started,
                    Ende = worklog.Started, // Worklogs haben nur ein Startdatum
                    Stunden = (double)worklog.TimeSpentSeconds / 3600,
                    Beschreibung = worklog.JiraProjekt,
                    InternBeschreibung = worklog.Comment,
                    Verrechenbar = projekte.First(p => p.Id == projektId).DefaultVerrechenbar == true,
                    JiraProjectId = worklog.JiraBuchungId
                });
            }
        }
        return leistungserfassungViewModels;
    }

    public static void Log(LogCategory category, string message)
    {
        DateTime now = DateTime.Now;
        if (category == LogCategory.LogfileInitialized)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    DateTime logStart = DateTime.Now;
                    sw.WriteLine($"[{logStart}]\tLogfile initialised\n");
                }
            }
        } else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine($"[{now}]\t[{(int)category}]\t{message}");
            }
        }

    }

    public static List<JiraProjectViewModel> GenerateJiraProjects(List<WhitelistViewModel> whitelist)
    {
        List<JiraProjectViewModel> jiraProjects = new List<JiraProjectViewModel>();
        foreach (var project in whitelist)
        {
            jiraProjects.Add(new JiraProjectViewModel
            {
                ProjectName = project.JiraProjectName,
                LeisProjectId = project.ProjektId
            });
        }
        return jiraProjects;
    }

    public static bool IsAuthorized(List<UserViewModel> userList, List<ProjektMitarbeiterViewModel> projektMitarbeiterList, string email, int projectId)
    {
        // Check if T_USER table contains a user with the given email adress
        if (!userList.Any(u => u.UniqueName == email))
        {
            Log(LogCategory.UserNotFound, $"User with email {email} could not be found.");
            return false;
        }

        // Check if TZ_PROJEKT_MITARBEITER contains a user with the id we got from T_USER and wether any of the entries with that id also contain the proper project id
        if (!projektMitarbeiterList.Any(pm => pm.MitarbeiterId == userList.First(u => u.UniqueName == email).MitarbeiterId && pm.ProjektId == projectId))
        {
            Log(LogCategory.UserNotAuthorized, $"User with email {email} is not authorized to book on project with Id {projectId}.");
            return false;
        }

        return true;
    }
}