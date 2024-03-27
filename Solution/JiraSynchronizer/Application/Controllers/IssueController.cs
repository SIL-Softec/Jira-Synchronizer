using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using JiraSynchronizer.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.Controllers;

public class IssueController
{
    private readonly IJiraRepository _jiraRepository;
    public IssueController()
    {
        _jiraRepository = new JiraRepository();
    }

    public async Task<List<IssueViewModel>> GetIssuesAsync(string projectName)
    {
        List<Issue> issues = await _jiraRepository.GetIssuesAsync(projectName);
        List<IssueViewModel> issueViewModels = new List<IssueViewModel>();
        foreach (Issue issue in issues)
        {
            issueViewModels.Add(new IssueViewModel()
            {
                IssueName = issue.IssueName
            });
        }
        return issueViewModels;
    }
}
