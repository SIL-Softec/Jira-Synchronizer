using FluentAssertions;
using JiraSynchronizer.Application;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests;

public class ProgramTest
{
    [Fact]
    public void GenerateJiraProjects__ImportWhitelistTableFromDatabase__GenerateViewModelsFromReceivedData()
    {
        //// Arrange
        //List<List<string>> whitelistReturnValue = [["1", "SWEPROJ"], ["2", "PROJECT"]];
        //var whitelistService = Substitute.For<IWhitelistService>();
        //whitelistService.GetWhitelist().Returns(whitelistReturnValue);

        //List<JiraProjectViewModel> expectedResult = new List<JiraProjectViewModel>()
        //{
        //    new JiraProjectViewModel()
        //    {
        //        ProjectName = "SWEPROJ",
        //        LeisProjectId = 1
        //    },
        //    new JiraProjectViewModel()
        //    {
        //        ProjectName = "PROJECT",
        //        LeisProjectId = 2
        //    }
        //};

        //// Act
        //var result = Program.GenerateJiraProjects(whitelistService.GetWhitelist());

        //// Assert
        //result.Should().BeEquivalentTo(expectedResult);
    }

    // Hätte gerne eine [Theory] verwendet, leider werden zweidimensionale string arrays jedoch als Argumente nicht akzeptiert, deshalb vier minimal verschiedene [Fact]
    [Fact]
    public void IsAuthorized__TestsIfUserWithSpecifiedEmailHasRightsOnSpecifiedProject__ReturnsFalseDueToNonExistantUser()
    {
        // Arrange
        // TODO test for log!
        //List<List<string>> userList = new List<List<string>>()
        //{
        //    new List<string>()
        //    {
        //        "email@gmail.com",
        //        "1"
        //    },
        //    new List<string>()
        //    {
        //        "test@gmail.com",
        //        "2"
        //    }
        //};
        //string email = "noneoftheabove@gmx.ch";

        //// Act
        //bool result = Program.IsAuthorized(userList, [], email, 1);

        //// Assert
        //result.Should().BeFalse();
    }

    [Fact]
    public void IsAuthorized__TestsIfUserWithSpecifiedEmailHasRightsOnSpecifiedProject__ReturnsFalseDueToMissingProjectUserMapping()
    {
        //// Arrange
        //// TODO test for log!
        //List<List<string>> userList = new List<List<string>>()
        //{
        //    new List<string>()
        //    {
        //        "email@gmail.com",
        //        "1"
        //    },
        //    new List<string>()
        //    {
        //        "test@gmail.com",
        //        "2"
        //    }
        //};
        //List<List<string>> projektMitarbeiterList = new List<List<string>>()
        //{
        //    new List<string>()
        //    {
        //        "1",
        //        "2"
        //    },
        //    new List<string>()
        //    {
        //        "3",
        //        "4"
        //    }
        //};
        //string email = "email@gmail.com";
        //int projectId = 1;

        //// Act
        //bool result = Program.IsAuthorized(userList, projektMitarbeiterList, email, projectId);

        //// Assert
        //result.Should().BeFalse();
    }

    [Fact]
    public void IsAuthorized__TestsIfUserWithSpecifiedEmailHasRightsOnSpecifiedProject__ReturnsFalseDueToNonexistantProject()
    {
        //// Arrange
        //// TODO test for log!
        //List<List<string>> userList = new List<List<string>>()
        //{
        //    new List<string>()
        //    {
        //        "email@gmail.com",
        //        "2"
        //    },
        //    new List<string>()
        //    {
        //        "test@gmail.com",
        //        "3"
        //    }
        //};
        //List<List<string>> projektMitarbeiterList = new List<List<string>>()
        //{
        //    new List<string>()
        //    {
        //        "1",
        //        "2"
        //    },
        //    new List<string>()
        //    {
        //        "3",
        //        "4"
        //    }
        //};
        //string email = "email@gmail.com";
        //int projectId = 9;

        //// Act
        //bool result = Program.IsAuthorized(userList, projektMitarbeiterList, email, projectId);

        //// Assert
        //result.Should().BeFalse();
    }

    [Fact]
    public void IsAuthorized__TestsIfUserWithSpecifiedEmailHasRightsOnSpecifiedProject__ReturnsTrue()
    {
        //// Arrange
        //// TODO test for log!
        //List<List<string>> userList = new List<List<string>>()
        //{
        //    new List<string>()
        //    {
        //        "email@gmail.com",
        //        "2"
        //    },
        //    new List<string>()
        //    {
        //        "test@gmail.com",
        //        "3"
        //    }
        //};
        //List<List<string>> projektMitarbeiterList = new List<List<string>>()
        //{
        //    new List<string>()
        //    {
        //        "1",
        //        "2"
        //    },
        //    new List<string>()
        //    {
        //        "3",
        //        "4"
        //    }
        //};
        //string email = "email@gmail.com";
        //int projectId = 1;

        //// Act
        //bool result = Program.IsAuthorized(userList, projektMitarbeiterList, email, projectId);

        //// Assert
        //result.Should().BeTrue();
    }
}
