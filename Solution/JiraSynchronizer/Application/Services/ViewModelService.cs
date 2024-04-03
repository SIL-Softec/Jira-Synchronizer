using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Enums;
using JiraSynchronizer.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.Services;

public class ViewModelService
{
    public List<LeistungserfassungViewModel> GenerateLeistungserfassungViewModels(List<WorklogViewModel> worklogs, List<UserViewModel> userList, List<JiraProjectViewModel> jiraProjects, List<ProjektViewModel> projekte, int? devUser, bool isDevelopment = false)
    {
        LoggingService logService = new LoggingService();
        List<LeistungserfassungViewModel> leistungserfassungViewModels = new List<LeistungserfassungViewModel>();
        foreach (WorklogViewModel worklog in worklogs)
        {
            JiraProjectViewModel jiraProject = jiraProjects.FirstOrDefault(w => w.ProjectName == worklog.JiraProjekt);
            int? projektId = jiraProject != null ? jiraProject.LeisProjectId : null;
            if (projektId != null)
            {
                leistungserfassungViewModels.Add(new LeistungserfassungViewModel()
                {
                    ProjektId = projektId.Value,
                    MitarbeiterId = !isDevelopment ? userList.First(u => u.UniqueName == worklog.Email).MitarbeiterId : devUser != null ? devUser.Value : -1,
                    Beginn = worklog.Started,
                    Ende = worklog.Started, // Worklogs haben nur ein Startdatum
                    Stunden = (double)worklog.TimeSpentSeconds / 3600,
                    Beschreibung = worklog.JiraProjekt,
                    InternBeschreibung = worklog.Comment,
                    Verrechenbar = projekte.First(p => p.Id == projektId).DefaultVerrechenbar == true,
                    JiraProjectId = !isDevelopment ? worklog.JiraBuchungId : null
                });
            } else
            {
                logService.Log(LogCategory.Error, "JiraProject enthält keine JiraProject Id");
            }
        }
        return leistungserfassungViewModels;
    }

    public List<JiraProjectViewModel> GenerateJiraProjectViewModels(List<WhitelistViewModel> whitelist)
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
}
