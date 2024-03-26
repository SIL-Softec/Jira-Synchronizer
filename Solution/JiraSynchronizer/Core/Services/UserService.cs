using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Services;

public class UserService
{
    private readonly SqlConnection _connection;

    public UserService(SqlConnection connection)
    {
        _connection = connection;
    }

    public List<List<string>> GetUsers()
    {
        SqlCommand getAllUsers = new SqlCommand("SELECT USR_UNIQUE_NAME, USR_MIT_ID FROM T_USER;", _connection);
        SqlDataReader reader = getAllUsers.ExecuteReader();
        List<List<string>> users = new List<List<string>>();

        while (reader.Read())
        {
            List<string> user = new List<string>();
            user.Add(reader[0].ToString());
            user.Add(reader[1].ToString());
            users.Add(user);
        }

        reader.Close();
        return users;
    }
}
