using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class ProjektMitarbeiterRepository : EfRepository<ProjektMitarbeiter>, IDatabaseRepository<ProjektMitarbeiter>
{
    public ProjektMitarbeiterRepository(SqlConnection connection) : base(connection) { }

    public List<ProjektMitarbeiter> ListAll()
    {
        SqlCommand getAllProjektMitarbeiterEntries = new SqlCommand("SELECT PMA_PRJ_ID, PMA_MIT_ID FROM TZ_PROJEKT_MITARBEITER;", Connection);
        SqlDataReader reader = getAllProjektMitarbeiterEntries.ExecuteReader();
        List<ProjektMitarbeiter> projektMitarbeiterList = new List<ProjektMitarbeiter>();

        while (reader.Read())
        {
            projektMitarbeiterList.Add(new ProjektMitarbeiter()
            {
                ProjektId = (int)reader[0],
                MitarbeiterId = (int)reader[1]
            });
        }

        reader.Close();
        return projektMitarbeiterList;
    }
}
