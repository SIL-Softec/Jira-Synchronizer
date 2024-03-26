using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Dto;
using JiraSynchronizer.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.Services;

public class JiraManagerService
{
    JiraService jiraService = new JiraService();

    public async Task<List<IssueViewModel>> GenerateIssues(string project)
    {
        List<IssueViewModel> issues = new List<IssueViewModel>();
        List<string> issueStrings = await jiraService.GetIssues(project);
        foreach (string issue in issueStrings)
        {
            issues.Add(new IssueViewModel()
            {
                IssueName = issue
            });
        }
        return issues;
    }

    public async Task<List<WorklogViewModel>> GenerateWorklogs(string issueName)
    {
        List<WorklogViewModel> worklogs = new List<WorklogViewModel>();
        List<WorklogDto> worklogDtos = await jiraService.GetWorklogs(issueName);
        foreach (WorklogDto worklogDto in worklogDtos)
        {
            worklogs.Add(new WorklogViewModel()
            {
                Email = worklogDto.Email,
                Started = worklogDto.Started,
                TimeSpentSeconds = worklogDto.TimeSpentSeconds,
                Comment = worklogDto.Comment,
                JiraBuchungId = worklogDto.JiraBuchungId
            });
        }
        return worklogs;
    }
}
