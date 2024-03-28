using FluentAssertions;
using JiraSynchronizer.Application.Controllers;
using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests;

public class WorklogControllerTest
{
    [Fact]
    public async void GetWorklogsAsync__GetDataFromRepository__ReturnListOfViewModelsOfData()
    {
        // Arrange
        IJiraRepository jiraRepository = Substitute.For<IJiraRepository>();
        WorklogController worklogController = new WorklogController(jiraRepository);
        string projectName = "JIRASYNC";

        List<Worklog> worklogs = new List<Worklog>()
        {
            new Worklog()
            {
                Email = "john.smith@gmail.com",
                Started = new DateTime(2024, 1, 1),
                TimeSpentSeconds = 3600, 
                Comment = "Worked really hard today",
                JiraBuchungId = 1,
                JiraProjekt = "JIRASYNC-1",
                IsAuthorized = false,
                ExistsOnDatabase = false
            }
        };

        List<WorklogViewModel> expectedResult = new List<WorklogViewModel>()
        {
            new WorklogViewModel()
            {
                Email = "john.smith@gmail.com",
                Started = new DateTime(2024, 1, 1),
                TimeSpentSeconds = 3600,
                Comment = "Worked really hard today",
                JiraBuchungId = 1,
                JiraProjekt = "JIRASYNC-1",
                IsAuthorized = false,
                ExistsOnDatabase = false
            }
        };
        jiraRepository.GetWorklogsAsync(projectName).Returns(worklogs);


        // Act
        List<WorklogViewModel> result = await worklogController.GetWorklogsAsync(projectName);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
