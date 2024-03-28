using FluentAssertions;
using JiraSynchronizer.Application.Services;
using JiraSynchronizer.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests;

public class AuthorizationServiceTest
{
    [Theory]
    [InlineData("john.smith@gmail.com", 2, true)]
    [InlineData("john.smith@gmail.com", 3, true)]
    [InlineData("nathanial.daniels@outlook.com", 2, false)]
    [InlineData("fred.bloggs@gmx.ch", 2, false)]
    public void IsAuthorized__InputsToValidateGivenUserEmail__ReturnsAuthorizationBoolean(string email, int projectId, bool expectedResult)
    {
        // Arrange
        AuthorizationService authService = new AuthorizationService();
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
        List<ProjektMitarbeiterViewModel> projektMitarbeiterList = new List<ProjektMitarbeiterViewModel>()
        {
            new ProjektMitarbeiterViewModel()
            {
                MitarbeiterId = 1,
                ProjektId = 2
            },
            new ProjektMitarbeiterViewModel()
            {
                MitarbeiterId = 1,
                ProjektId = 3
            }
        };

        // Act
        bool result = authService.IsAuthorized(userList, projektMitarbeiterList, email, projectId);

        // Assert
        result.Should().Be(expectedResult);
    }
}
