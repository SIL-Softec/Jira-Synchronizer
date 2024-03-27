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

public class WorklogController
{
    private readonly IJiraRepository _jiraRepository;
    public WorklogController()
    {
        _jiraRepository = new JiraRepository();
    }

    public async Task<List<WorklogViewModel>> GetWorklogsAsync(string issueName)
    {
        List<Worklog> worklogs = await _jiraRepository.GetWorklogsAsync(issueName);
        List<WorklogViewModel> worklogViewModels = new List<WorklogViewModel>();
        foreach (Worklog worklog in worklogs)
        {
            worklogViewModels.Add(new WorklogViewModel()
            {
                Email = worklog.Email,
                Started = worklog.Started,
                TimeSpentSeconds = worklog.TimeSpentSeconds,
                Comment = worklog.Comment,
                JiraBuchungId = worklog.JiraBuchungId,
                JiraProjekt = worklog.JiraProjekt,
                IsAuthorized = worklog.IsAuthorized,
                ExistsOnDatabase = worklog.ExistsOnDatabase
            });
        }
        return worklogViewModels;
    }
}
