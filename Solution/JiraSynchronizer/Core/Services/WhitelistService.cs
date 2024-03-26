using JiraSynchronizer.Core.Interfaces.Repositories;
using JiraSynchronizer.Core.Interfaces.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Services;

public class WhitelistService : IWhitelistService
{
    private readonly SqlConnection _connection;

    public WhitelistService(SqlConnection connection)
    {
        _connection = connection;
    }

    public List<List<string>> GetWhitelist()
    {
        SqlCommand getAllWhitelists = new SqlCommand("SELECT * FROM T_WHITELIST;", _connection);
        SqlDataReader reader = getAllWhitelists.ExecuteReader();
        List<List<string>> whitelist = new List<List<string>>();

        while (reader.Read())
        {
            List<string> whitelistItem = new List<string>();
            whitelistItem.Add(reader[1].ToString());
            whitelistItem.Add(reader[2].ToString());
            whitelist.Add(whitelistItem);
        }

        reader.Close();
        return whitelist;
    }
}
