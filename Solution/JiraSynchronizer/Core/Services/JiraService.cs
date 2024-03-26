using JiraSynchronizer.Core.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Services;

public class JiraService
{
    static HttpClient client = new HttpClient();
    static string authorization = ConfigurationManager.AppSettings["api_token"] ?? "Not Found";

    public async Task<List<string>> GetIssues(string jiraProject)
    {
        if (authorization == "Not Found") throw new UnauthorizedAccessException();

        // Generate Request URL
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://softec-swe.atlassian.net/rest/api/3/search?jql=project={jiraProject}&maxResults=1000");

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
        List<string> issues = new List<string>();
        foreach (dynamic issue in jsonIssues)
        {
            string key = issue.key;
            issues.Add(key);
        }

        return issues;
    }

    public async Task<List<WorklogDto>> GetWorklogs(string issueName)
    {
        if (authorization == "Not Found") throw new UnauthorizedAccessException();

        // Generate Request URL
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://softec-swe.atlassian.net/rest/api/3/issue/{issueName}/worklog");

        // Set Authorization Header
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authorization)));

        // Receive Response and make sure it's valid
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // Generate json Objects based on response
        string responseJson = await response.Content.ReadAsStringAsync();
        dynamic jsonObject = JsonConvert.DeserializeObject(responseJson);
        dynamic jsonWorklogs = jsonObject.worklogs;

        List<WorklogDto> worklogDtos = new List<WorklogDto>();

        if (jsonWorklogs.Count > 0)
        {
            DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);
            foreach (dynamic jsonWorklog in jsonWorklogs)
            {
                DateTime worklogStarted = (DateTime)jsonWorklog.started;
                if (worklogStarted >= sevenDaysAgo)
                {
                    WorklogDto worklogDto = new WorklogDto()
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
                            worklogDto.Comment = jsonWorklog.comment.content[0].content[0].text;
                        }
                    }
                    worklogDtos.Add(worklogDto);
                }
            }
        }

        return worklogDtos;
    }
}
