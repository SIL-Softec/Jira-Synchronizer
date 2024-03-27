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

public class ProjektController : BaseController
{
    private readonly IDatabaseRepository<Projekt> _databaseRepository;
    public ProjektController(SqlConnection connection) : base(connection)
    {
        _databaseRepository = new ProjektRepository(connection);
    }

    public List<ProjektViewModel> GetAllProjekte()
    {
        List<Projekt> projekte = _databaseRepository.ListAll();
        List<ProjektViewModel> projektViewModels = new List<ProjektViewModel>();
        foreach (Projekt projekt in projekte)
        {
            projektViewModels.Add(new ProjektViewModel()
            {
                Id = projekt.Id,
                DefaultVerrechenbar = projekt.DefaultVerrechenbar
            });
        }
        return projektViewModels;
    }
}
