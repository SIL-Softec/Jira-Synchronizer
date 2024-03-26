using JiraSynchronizer.Application.Services;
using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Interfaces.Services;
using JiraSynchronizer.Core.Services;
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
        JiraManagerService jiraManagerService = new JiraManagerService();
        sqlConnection.Open();
        WhitelistService whitelistService = new WhitelistService(sqlConnection);
        UserService userService = new UserService(sqlConnection);
        ProjektMitarbeiterService projektMitarbeiterService = new ProjektMitarbeiterService(sqlConnection);

        // Initialise Log 
        StartLog();

        // Whitelist wird von der Datenbank importiert und in JiraProjectViewModel übersetzt
        List<JiraProjectViewModel> jiraProjects = GenerateJiraProjects(whitelistService.GetWhitelist());

        // Alle issue keys der jeweiligen Projekte werden von der Jira REST API importiert
        foreach (JiraProjectViewModel project in jiraProjects)
        {
            project.Issues = await jiraManagerService.GenerateIssues(project.ProjectName);
        }

        // Alle worklogs, jünger als sieben Tage, der jeweiligen Issues werden von der Jira REST API importiert
        foreach (JiraProjectViewModel project in jiraProjects)
        {
            foreach (IssueViewModel issue in project.Issues)
            {
                issue.Worklogs = await jiraManagerService.GenerateWorklogs(issue.IssueName);
            }
        }

        // Bei allen worklogs wird überprüft, ob der erfasste Benutzer berechtigt ist, auf das jeweilige Projekt zu buchen
        List<List<string>> userList = userService.GetUsers();
        List<List<string>> projektMitarbeiterList = projektMitarbeiterService.GetProjektMitarbeiterList();

        foreach (JiraProjectViewModel project in jiraProjects)
        {
            foreach (IssueViewModel issue in project.Issues)
            {
                foreach (WorklogViewModel worklog in issue.Worklogs)
                {
                    worklog.IsAuthorized = IsAuthorized(userList, projektMitarbeiterList, worklog.Email, project.LeisProjectId);
                    Console.WriteLine(project.LeisProjectId);
                    Console.WriteLine(worklog.Email);
                    Console.WriteLine(worklog.IsAuthorized);
                }
            }
        }



        sqlConnection.Close();
        Console.ReadKey();
    }

    public static void StartLog()
    {
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                DateTime logStart = DateTime.Now;
                sw.WriteLine($"[{logStart}]\tLogfile initialised\n");
            }
        }
        using (StreamWriter sw = File.AppendText(path))
        {
            DateTime applicationStart = DateTime.Now;
            sw.WriteLine($"[{applicationStart}]\tApplication started\n");
        }
    }

    public static List<JiraProjectViewModel> GenerateJiraProjects(List<List<string>> whitelist)
    {
        List<JiraProjectViewModel> jiraProjects = new List<JiraProjectViewModel>();
        foreach (var project in whitelist)
        {
            jiraProjects.Add(new JiraProjectViewModel
            {
                ProjectName = project[1],
                LeisProjectId = Int32.Parse(project[0])
            });
        }
        return jiraProjects;
    }

    public static bool IsAuthorized(List<List<string>> userList, List<List<string>> projektMitarbeiterList, string email, int projectId)
    {
        // TODO: On false write error into log!!!

        // Check if T_USER table contains a user with the given email adress
        if (!userList.Any(u => u[0] == email))
        {
            return false;
        }

        // TODO: On false write error into log!!!
        // Check if TZ_PROJEKT_MITARBEITER contains a user with the id we got from T_USER and wether any of the entries with that id also contain the proper project id
        if (!projektMitarbeiterList.Any(pm => pm[1] == userList.First(u => u[0] == email)[1] && pm[0] == projectId.ToString()))
        {
            return false;
        }

        return true;
    }
}