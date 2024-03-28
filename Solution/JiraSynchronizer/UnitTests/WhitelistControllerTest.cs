using FluentAssertions;
using JiraSynchronizer.Application.Controllers;
using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using Microsoft.Data.SqlClient;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests;

public class WhitelistControllerTest
{
    [Fact]
    public void GetAllWhitelists__ListWhitelistsInDatabase__ReturnWhitelistsVMsYoungerThanSevenDays()
    {
        // Arrange
        IDatabaseRepository<Whitelist> databaseRepository = Substitute.For<IDatabaseRepository<Whitelist>>();
        SqlConnection connection = new SqlConnection("");
        WhitelistController whitelistController = new WhitelistController(databaseRepository, connection);

        List<Whitelist> whitelists = new List<Whitelist>()
        {
            new Whitelist()
            {
                ProjektId = 1,
                JiraProjectName = "JIRASYNC"
            }
        };
        databaseRepository.ListAll().Returns(whitelists);
        List<WhitelistViewModel> expectedResult = new List<WhitelistViewModel>()
        {
            new WhitelistViewModel()
            {
                ProjektId = 1,
                JiraProjectName = "JIRASYNC"
            }
        };

        // Act
        List<WhitelistViewModel> result = whitelistController.GetAllWhitelists();

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
