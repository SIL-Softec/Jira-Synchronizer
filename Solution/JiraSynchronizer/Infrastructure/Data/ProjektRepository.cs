using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class ProjektRepository : EfRepository<Projekt>, IDatabaseRepository<Projekt>
{
    public ProjektRepository(SqlConnection connection) : base(connection) { }

    public List<Projekt> ListAll()
    {
        SqlCommand getAllProjekte = new SqlCommand("SELECT PRJ_ID, PRJ_DEFAULTVERRECHENBAR FROM T_PROJEKT RIGHT JOIN T_WHITELIST ON T_WHITELIST.WHL_PRJ_ID = PRJ_ID;", Connection);
        SqlDataReader reader = getAllProjekte.ExecuteReader();
        List<Projekt> projekte = new List<Projekt>();

        while (reader.Read())
        {
            projekte.Add(new Projekt()
            {
                Id = (int)reader[0],
                DefaultVerrechenbar = reader[1] != DBNull.Value ? (bool)reader[1] : false
            });
        }

        reader.Close();
        return projekte;
    }
}
