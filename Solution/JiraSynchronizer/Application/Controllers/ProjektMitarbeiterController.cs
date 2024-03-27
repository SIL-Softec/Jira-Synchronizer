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

public class ProjektMitarbeiterController : BaseController
{
    private readonly IDatabaseRepository<ProjektMitarbeiter> _databaseRepository;
    public ProjektMitarbeiterController(SqlConnection connection) : base(connection)
    {
        _databaseRepository = new ProjektMitarbeiterRepository(connection);
    }

    public List<ProjektMitarbeiterViewModel> GetAllProjektMitarbeiters()
    {
        List<ProjektMitarbeiter> projektMitarbeiters = _databaseRepository.ListAll();
        List<ProjektMitarbeiterViewModel> projektMitarbeiterViewModels = new List<ProjektMitarbeiterViewModel>();
        foreach (ProjektMitarbeiter projektMitarbeiter in projektMitarbeiters)
        {
            projektMitarbeiterViewModels.Add(new ProjektMitarbeiterViewModel()
            {
                ProjektId = projektMitarbeiter.ProjektId,
                MitarbeiterId = projektMitarbeiter.MitarbeiterId
            });
        }
        return projektMitarbeiterViewModels;
    }
}
