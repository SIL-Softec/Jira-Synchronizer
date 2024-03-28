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

public class ProjektControllerTest
{
    [Fact]
    public void GetAllProjekte__ListProjekteInDatabase__ReturnProjekteVMsYoungerThanSevenDays()
    {
        // Arrange
        IDatabaseRepository<Projekt> databaseRepository = Substitute.For<IDatabaseRepository<Projekt>>();
        SqlConnection connection = new SqlConnection("");
        ProjektController projektController = new ProjektController(databaseRepository, connection);

        List<Projekt> projekte = new List<Projekt>()
        {
            new Projekt()
            {
                Id = 1,
                DefaultVerrechenbar = true
            }
        };
        databaseRepository.ListAll().Returns(projekte);
        List<ProjektViewModel> expectedResult = new List<ProjektViewModel>()
        {
            new ProjektViewModel()
            {
                Id = 1,
                DefaultVerrechenbar = true
            }
        };

        // Act
        List<ProjektViewModel> result = projektController.GetAllProjekte();

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
