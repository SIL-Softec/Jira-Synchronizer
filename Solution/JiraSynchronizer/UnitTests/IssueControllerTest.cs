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

public class IssueControllerTest
{
    [Fact]
    public async void GetIssuesAsync__GetDataFromRepository__ReturnListOfViewModelsOfData()
    {
        // Arrange
        IJiraRepository jiraRepository = Substitute.For<IJiraRepository>();
        IssueController issueController = new IssueController(jiraRepository);
        string projectName = "JIRASYNC";

        List<Issue> issues = new List<Issue>()
        {
            new Issue()
            {
                IssueName = "JIRASYNC-1"
            }
        };

        List<IssueViewModel> expectedResult = new List<IssueViewModel>()
        {
            new IssueViewModel()
            {
                IssueName = "JIRASYNC-1"
            }
        };
        jiraRepository.GetIssuesAsync(projectName).Returns(issues);


        // Act
        List<IssueViewModel> result = await issueController.GetIssuesAsync(projectName);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
