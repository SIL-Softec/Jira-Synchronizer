using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Services;

public class ProjektMitarbeiterService
{
    private readonly SqlConnection _connection;

    public ProjektMitarbeiterService(SqlConnection connection)
    {
        _connection = connection;
    }

    public List<List<string>> GetProjektMitarbeiterList()
    {
        SqlCommand getAllProjektMitarbeiterEntries = new SqlCommand("SELECT PMA_PRJ_ID, PMA_MIT_ID FROM TZ_PROJEKT_MITARBEITER;", _connection);
        SqlDataReader reader = getAllProjektMitarbeiterEntries.ExecuteReader();
        List<List<string>> projektMitarbeiterList = new List<List<string>>();

        while (reader.Read())
        {
            List<string> projektMitarbeiter = new List<string>();
            projektMitarbeiter.Add(reader[0].ToString());
            projektMitarbeiter.Add(reader[1].ToString());
            projektMitarbeiterList.Add(projektMitarbeiter);
        }

        reader.Close();
        return projektMitarbeiterList;
    }
}
