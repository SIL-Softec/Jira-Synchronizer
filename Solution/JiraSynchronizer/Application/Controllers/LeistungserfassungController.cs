using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using JiraSynchronizer.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.Controllers;

public class LeistungserfassungController : BaseController
{
    private readonly IDatabaseRepository<Leistungserfassung> _databaseRepository;
    public LeistungserfassungController(SqlConnection connection) : base(connection)
    {
        _databaseRepository = new LeistungserfassungRepository(connection);
    }

    // Alternate constructor to mock database repository in tests
    public LeistungserfassungController(IDatabaseRepository<Leistungserfassung> databaseRepository, SqlConnection connection) : base(connection)
    {
        _databaseRepository = databaseRepository;
    }

    public List<LeistungserfassungViewModel> GetAllLeistungserfassungen()
    {
        List<Leistungserfassung> leistungserfassungen = _databaseRepository.ListAll();
        List<LeistungserfassungViewModel> leistungserfassungViewModels = new List<LeistungserfassungViewModel>();
        foreach (Leistungserfassung leistungserfassung in leistungserfassungen)
        {
            leistungserfassungViewModels.Add(new LeistungserfassungViewModel()
            {
                ProjektId = leistungserfassung.ProjektId,
                MitarbeiterId = leistungserfassung.MitarbeiterId,
                JiraProjectId = leistungserfassung.JiraProjectId,
                Beginn = leistungserfassung.Beginn,
                Ende = leistungserfassung.Ende,
                Stunden = leistungserfassung.Stunden,
                Verrechenbar = leistungserfassung.Verrechenbar,
                Beschreibung = leistungserfassung.Beschreibung,
                InternBeschreibung = leistungserfassung.InternBeschreibung
            });
        }
        return leistungserfassungViewModels;
    }

    public void AddLeistungserfassungen(List<LeistungserfassungViewModel> leistungserfassungViewModels)
    {
        List<Leistungserfassung> leistungserfassungen = new List<Leistungserfassung>();
        foreach (LeistungserfassungViewModel leistungserfassung in leistungserfassungViewModels)
        {
            leistungserfassungen.Add(new Leistungserfassung()
            {
                ProjektId = leistungserfassung.ProjektId,
                MitarbeiterId = leistungserfassung.MitarbeiterId,
                JiraProjectId = leistungserfassung.JiraProjectId,
                Beginn = leistungserfassung.Beginn,
                Ende = leistungserfassung.Ende,
                Stunden = leistungserfassung.Stunden,
                Verrechenbar = leistungserfassung.Verrechenbar,
                Beschreibung = leistungserfassung.Beschreibung,
                InternBeschreibung = leistungserfassung.InternBeschreibung
            });
        }
        _databaseRepository.AddAll(leistungserfassungen);
    }
}
