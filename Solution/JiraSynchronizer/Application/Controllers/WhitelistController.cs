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

public class WhitelistController : BaseController
{
    private readonly IDatabaseRepository<Whitelist> _databaseRepository;
    public WhitelistController(SqlConnection connection) : base(connection)
    {
        _databaseRepository = new WhitelistRepository(connection);
    }

    public List<WhitelistViewModel> GetAllWhitelists()
    {
        List<Whitelist> whitelists = _databaseRepository.ListAll();
        List<WhitelistViewModel> whitelistViewModels = new List<WhitelistViewModel>();
        foreach (Whitelist whitelist in whitelists)
        {
            whitelistViewModels.Add(new WhitelistViewModel()
            {
                ProjektId = whitelist.ProjektId,
                JiraProjectName = whitelist.JiraProjectName
            });
        }
        return whitelistViewModels;
    }
}
