using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class LeistungserfassungRepository : EfRepository<Leistungserfassung>, IDatabaseRepository<Leistungserfassung> 
{
    public LeistungserfassungRepository(SqlConnection connection) : base(connection) { }

    public List<Leistungserfassung> ListAll()
    {
        DateTime sevenDaysAgoDate = DateTime.Today.AddDays(-7);
        string sevenDaysAgoDateString = $"{sevenDaysAgoDate.Year}-{sevenDaysAgoDate.Month}-{sevenDaysAgoDate.Day} 00:00:00";
        SqlCommand getLastSevenDaysLeistungserfassungen = new SqlCommand($"SELECT LEI_BEGINN, LEI_JIRA_BUCHUNG_ID FROM T_LEISTUNGSERFASSUNG WHERE T_LEISTUNGSERFASSUNG.LEI_BEGINN >= '{sevenDaysAgoDateString}';", Connection);
        SqlDataReader reader = getLastSevenDaysLeistungserfassungen.ExecuteReader();
        List<Leistungserfassung> leistungserfassungen = new List<Leistungserfassung>();

        while (reader.Read())
        {
            Leistungserfassung leistungserfassung = new Leistungserfassung()
            {
                Beginn = DateTime.Parse(reader[0].ToString()),
                JiraProjectId = reader[1] != DBNull.Value ? (long)reader[1] : null
            };
            leistungserfassungen.Add(leistungserfassung);
        }

        reader.Close();
        return leistungserfassungen;
    }

    public void AddAll(List<Leistungserfassung> leistungserfassungen)
    {
        if (leistungserfassungen.Count() == 0)
        {
            return;
        }
        string sqlCommand = "INSERT INTO T_LEISTUNGSERFASSUNG (LEI_PRJ_ID, LEI_MIT_ID, LEI_LEA_ID, LEI_ZKL_ID, LEI_BEGINN, LEI_ENDE, LEI_STUNDEN, LEI_VERRECHENBAR, LEI_BESCHREIBUNG, LEI_INTERN_BESCHREIBUNG, LEI_MIA_SICHTBAR, LEI_JIRA_BUCHUNG_ID) VALUES ";
        for (int i = 0; i < leistungserfassungen.Count(); i++)
        {
            if (i != 0)
            {
                sqlCommand += ", ";
            }
            int verrechenbar = leistungserfassungen[i].Verrechenbar == true ? 1 : 0;
            string beginnDate = leistungserfassungen[i].Beginn.ToString("yyyy-MM-dd hh:mm:ss");
            string endeDate = leistungserfassungen[i].Ende.ToString("yyyy-MM-dd hh:mm:ss");
            sqlCommand += $"({leistungserfassungen[i].ProjektId}, {leistungserfassungen[i].MitarbeiterId}, 10, 1, '{beginnDate}', '{endeDate}', {leistungserfassungen[i].Stunden}, {verrechenbar}, '{leistungserfassungen[i].Beschreibung}', '{leistungserfassungen[i].InternBeschreibung}', 1, {leistungserfassungen[i].JiraProjectId})";
        }
        sqlCommand += ";";
        SqlCommand insertLeistungserfassungen = new SqlCommand(sqlCommand, Connection);
        insertLeistungserfassungen.ExecuteNonQuery();
    }
}
