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

public class UserControllerTest
{
    [Fact]
    public void GetAllUsers__ListUsersInDatabase__ReturnUsersVMsYoungerThanSevenDays()
    {
        // Arrange
        IDatabaseRepository<User> databaseRepository = Substitute.For<IDatabaseRepository<User>>();
        SqlConnection connection = new SqlConnection("");
        UserController userController = new UserController(databaseRepository, connection);

        List<User> users = new List<User>()
        {
            new User()
            {
                MitarbeiterId = 1,
                UniqueName = "Max Muster"
            }
        };
        databaseRepository.ListAll().Returns(users);
        List<UserViewModel> expectedResult = new List<UserViewModel>()
        {
            new UserViewModel()
            {
                MitarbeiterId = 1,
                UniqueName = "Max Muster"
            }
        };

        // Act
        List<UserViewModel> result = userController.GetAllUsers();

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
