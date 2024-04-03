using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using JiraSynchronizer.Core.Services;
using JiraSynchronizer.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.Controllers;

public class WorklogController
{
    private LoggingService logService = new LoggingService();
    private readonly IJiraRepository _jiraRepository;
    public WorklogController()
    {
        _jiraRepository = new JiraRepository();
    }

    // Alternate constructor to mock repository in tests
    public WorklogController(IJiraRepository jiraRepository)
    {
        _jiraRepository = jiraRepository;
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
            if (((double)worklog.TimeSpentSeconds) / 3600 > 24) logService.Log(Core.Enums.LogCategory.Warning, $"Worklog vom {worklog.Started} enthält eine Buchung von über 24 Stunden");
        }
        return worklogViewModels;
    }
}
