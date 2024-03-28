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

public class ProjektMitarbeiterControllerTest
{
    [Fact]
    public void GetAllProjektMitarbeiters__ListProjektMitarbeitersInDatabase__ReturnProjektMitarbeitersVMsYoungerThanSevenDays()
    {
        // Arrange
        IDatabaseRepository<ProjektMitarbeiter> databaseRepository = Substitute.For<IDatabaseRepository<ProjektMitarbeiter>>();
        SqlConnection connection = new SqlConnection("");
        ProjektMitarbeiterController projektMitarbeiterController = new ProjektMitarbeiterController(databaseRepository, connection);

        List<ProjektMitarbeiter> projektMitarbeiters = new List<ProjektMitarbeiter>()
        {
            new ProjektMitarbeiter()
            {
                ProjektId = 1,
                MitarbeiterId = 2
            }
        };
        databaseRepository.ListAll().Returns(projektMitarbeiters);
        List<ProjektMitarbeiterViewModel> expectedResult = new List<ProjektMitarbeiterViewModel>()
        {
            new ProjektMitarbeiterViewModel()
            {
                ProjektId = 1,
                MitarbeiterId = 2
            }
        };

        // Act
        List<ProjektMitarbeiterViewModel> result = projektMitarbeiterController.GetAllProjektMitarbeiters();

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
