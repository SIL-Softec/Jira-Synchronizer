using FluentAssertions;
using JiraSynchronizer.Application.Services;
using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests;

public class ViewModelServiceTest
{
    [Fact]
    public void GenerateLeistungserfassungViewModels__ProcessData__LeistungserfassungViewModelFromData()
    {
        // Arrange
        ViewModelService viewModelService = new ViewModelService();
        List<WorklogViewModel> worklogs = new List<WorklogViewModel>()
        {
            new WorklogViewModel()
            {
                Email = "john.smith@gmail.com",
                Started = new DateTime(2024, 1, 1),
                TimeSpentSeconds = 3600,
                Comment = "Working on Project Jira Synchronizer",
                JiraBuchungId = 12345,
                JiraProjekt = "JIRASYNC",
                IsAuthorized = true,
                ExistsOnDatabase = true
            },
            new WorklogViewModel()
            {
                Email = "fred.bloggs@gmx.ch",
                Started = new DateTime(2024, 2, 2),
                TimeSpentSeconds = 1800,
                Comment = "Jira Synchronizer",
                JiraBuchungId = 67890,
                JiraProjekt = "JIRASYNC",
                IsAuthorized = false,
                ExistsOnDatabase = false
            },
            new WorklogViewModel()
            {
                Email = "nathanial.daniels@outlook.com",
                Started = new DateTime(2024, 3, 3),
                TimeSpentSeconds = 30000,
                Comment = "Private Project",
                JiraBuchungId = 13579,
                JiraProjekt = "NONREGISTERED",
                IsAuthorized = false,
                ExistsOnDatabase = false
            }
        };
        List<UserViewModel> userList = new List<UserViewModel>()
        {
            new UserViewModel()
            {
                UniqueName = "john.smith@gmail.com",
                MitarbeiterId = 1
            },
            new UserViewModel()
            {
                UniqueName = "fred.bloggs@gmx.ch",
                MitarbeiterId = 2
            }
        };
        List<JiraProjectViewModel> jiraProjects = new List<JiraProjectViewModel>()
        {
            new JiraProjectViewModel()
            {
                ProjectName = "JIRASYNC",
                LeisProjectId = 1
            }
        };
        List<ProjektViewModel> projects = new List<ProjektViewModel>()
        {
            new ProjektViewModel()
            {
                Id = 1,
                DefaultVerrechenbar = true
            }
        };

        List<LeistungserfassungViewModel> expectedResult = new List<LeistungserfassungViewModel>()
        {
            new LeistungserfassungViewModel()
            {
                Beginn = new DateTime(2024, 1, 1), 
                Beschreibung = "JIRASYNC", 
                Ende = new DateTime(2024, 1, 1), 
                InternBeschreibung = "Working on Project Jira Synchronizer", 
                JiraProjectId = 12345, 
                MitarbeiterId = 1, 
                ProjektId = 1, 
                Stunden = 1.0, 
                Verrechenbar = true
            },
            new LeistungserfassungViewModel()
            {
                Beginn = new DateTime(2024, 2, 2),
                Beschreibung = "JIRASYNC",
                Ende = new DateTime(2024, 2, 2),
                InternBeschreibung = "Jira Synchronizer",
                JiraProjectId = 67890,
                MitarbeiterId = 2,
                ProjektId = 1,
                Stunden = 0.5,
                Verrechenbar = true
            }
        };

        // Act
        List<LeistungserfassungViewModel> result = viewModelService.GenerateLeistungserfassungViewModels(worklogs, userList, jiraProjects, projects, null);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void GenerateJiraProjectViewModels__ProcessData__JiraProjectViewModelFromData()
    {
        // Arrange
        ViewModelService viewModelService = new ViewModelService();
        List<WhitelistViewModel> whitelist = new List<WhitelistViewModel>()
        {
            new WhitelistViewModel()
            {
                ProjektId = 1,
                JiraProjectName = "JIRASYNC"
            },
            new WhitelistViewModel()
            {
                ProjektId = 2,
                JiraProjectName = "NONREGISTERED"
            }
        };

        List<JiraProjectViewModel> expectedResult = new List<JiraProjectViewModel>()
        {
            new JiraProjectViewModel()
            {
                LeisProjectId = 1,
                ProjectName = "JIRASYNC"
            },
            new JiraProjectViewModel()
            {
                LeisProjectId = 2,
                ProjectName = "NONREGISTERED"
            }
        };

        // Act
        List<JiraProjectViewModel> result = viewModelService.GenerateJiraProjectViewModels(whitelist);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
