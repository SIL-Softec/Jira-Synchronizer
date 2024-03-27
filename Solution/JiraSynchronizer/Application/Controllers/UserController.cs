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

public class UserController : BaseController
{
    private readonly IDatabaseRepository<User> _databaseRepository;
    public UserController(SqlConnection connection) : base(connection)
    {
        _databaseRepository = new UserRepository(connection);
    }

    public List<UserViewModel> GetAllUsers()
    {
        List<User> users = _databaseRepository.ListAll();
        List<UserViewModel> userViewModels = new List<UserViewModel>();
        foreach (User user in users)
        {
            userViewModels.Add(new UserViewModel()
            {
                MitarbeiterId = user.MitarbeiterId,
                UniqueName = user.UniqueName
            });
        }
        return userViewModels;
    }
}
