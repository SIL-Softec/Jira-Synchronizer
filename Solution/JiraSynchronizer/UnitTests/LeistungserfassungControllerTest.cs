using FluentAssertions;
using JiraSynchronizer.Application.Controllers;
using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using JiraSynchronizer.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests;

public class LeistungserfassungControllerTest
{
    [Fact]
    public void GetAllLeistungserfassungen__ListLeistungserfassungenInDatabase__ReturnLeistungserfassungenVMsYoungerThanSevenDays()
    {
        // Arrange
        IDatabaseRepository<Leistungserfassung> databaseRepository = Substitute.For<IDatabaseRepository<Leistungserfassung>>();
        SqlConnection connection = new SqlConnection("");
        LeistungserfassungController leistungserfassungController = new LeistungserfassungController(databaseRepository, connection);

        List<Leistungserfassung> leistungserfassungen = new List<Leistungserfassung>()
        {
            new Leistungserfassung()
            {
                ProjektId = 1,
                MitarbeiterId = 2,
                JiraProjectId = 3,
                Beginn = new DateTime(2024, 1, 1),
                Ende = new DateTime(2024, 2, 2),
                Stunden = 4,
                Verrechenbar = true,
                Beschreibung = "JIRASYNC-1",
                InternBeschreibung = "Massive Produktivität"
            }
        };
        databaseRepository.ListAll().Returns(leistungserfassungen);
        List<LeistungserfassungViewModel> expectedResult = new List<LeistungserfassungViewModel>()
        {
            new LeistungserfassungViewModel()
            {
                ProjektId = 1,
                MitarbeiterId = 2,
                JiraProjectId = 3,
                Beginn = new DateTime(2024, 1, 1),
                Ende = new DateTime(2024, 2, 2),
                Stunden = 4,
                Verrechenbar = true,
                Beschreibung = "JIRASYNC-1",
                InternBeschreibung = "Massive Produktivität"
            }
        };

        // Act
        List<LeistungserfassungViewModel> result = leistungserfassungController.GetAllLeistungserfassungen();

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
