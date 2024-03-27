using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class UserRepository : EfRepository<User>, IDatabaseRepository<User>
{
    public UserRepository(SqlConnection connection) : base(connection) { }

    public List<User> ListAll()
    {
        SqlCommand getAllUsers = new SqlCommand("SELECT USR_UNIQUE_NAME, USR_MIT_ID FROM T_USER;", Connection);
        SqlDataReader reader = getAllUsers.ExecuteReader();
        List<User> users = new List<User>();

        while (reader.Read())
        {
            List<string> user = new List<string>();
            users.Add(new User()
            {
                UniqueName = reader[0].ToString(),
                MitarbeiterId = (int)reader[1]
            });
        }

        reader.Close();
        return users;
    }
}
