using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class WhitelistRepository : EfRepository<Whitelist>, IDatabaseRepository<Whitelist>
{
    public WhitelistRepository(SqlConnection connection) : base(connection) { }

    public List<Whitelist> ListAll()
    {
        SqlCommand getAllWhitelists = new SqlCommand("SELECT WHL_PRJ_ID, WHL_JIRA_PROJECT_NAME FROM T_WHITELIST;", Connection);
        SqlDataReader reader = getAllWhitelists.ExecuteReader();
        List<Whitelist> whitelist = new List<Whitelist>();

        while (reader.Read())
        {
            whitelist.Add(new Whitelist()
            {
                ProjektId = (int)reader[0],
                JiraProjectName = reader[1].ToString()
            });
        }

        reader.Close();
        return whitelist;
    }
}
