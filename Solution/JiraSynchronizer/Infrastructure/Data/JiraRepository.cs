using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class JiraRepository : IJiraRepository
{
    static HttpClient client = new HttpClient();
    static string authorization = ConfigurationManager.AppSettings["api_token"] ?? "Not Found";
    static readonly string baseUrl = "https://softec-swe.atlassian.net/rest/api/3";

    public async Task<List<Issue>> GetIssuesAsync(string jiraProject)
    {
        if (authorization == "Not Found") throw new UnauthorizedAccessException();

        // Generate Request URL
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/search?jql=project={jiraProject}&maxResults=1000");

        // Set Authorization Header
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authorization)));

        // Receive response and make sure it's valid
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // Generate json Objects based on response
        string responseJson = await response.Content.ReadAsStringAsync();
        dynamic jsonObject = JsonConvert.DeserializeObject(responseJson);
        dynamic jsonIssues = jsonObject.issues;

        // Fill issue list according to json objects
        List<Issue> issues = new List<Issue>();
        foreach (dynamic issue in jsonIssues)
        {
            issues.Add(new Issue()
            {
                IssueName = issue.key
            });
        }

        return issues;
    }

    public async Task<List<Worklog>> GetWorklogsAsync(string issueName)
    {
        if (authorization == "Not Found") throw new UnauthorizedAccessException();

        // Generate Request URL
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/issue/{issueName}/worklog");

        // Set Authorization Header
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authorization)));

        // Receive Response and make sure it's valid
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // Generate json Objects based on response
        string responseJson = await response.Content.ReadAsStringAsync();
        dynamic jsonObject = JsonConvert.DeserializeObject(responseJson);
        dynamic jsonWorklogs = jsonObject.worklogs;

        List<Worklog> worklogs = new List<Worklog>();

        if (jsonWorklogs.Count > 0)
        {
            DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);
            foreach (dynamic jsonWorklog in jsonWorklogs)
            {
                DateTime worklogStarted = (DateTime)jsonWorklog.started;
                if (worklogStarted >= sevenDaysAgo)
                {
                    Worklog worklog = new Worklog()
                    {
                        Email = jsonWorklog.author.emailAddress,
                        Started = worklogStarted,
                        TimeSpentSeconds = jsonWorklog.timeSpentSeconds,
                        JiraBuchungId = jsonWorklog.id
                    };
                    if (jsonWorklog.comment != null)
                    {
                        if (jsonWorklog.comment.content.Count > 0)
                        {
                            worklog.Comment = jsonWorklog.comment.content[0].content[0].text;
                        }
                    }
                    worklogs.Add(worklog);
                }
            }
        }

        return worklogs;
    }
}
