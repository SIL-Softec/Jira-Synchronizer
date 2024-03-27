using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Entities;

public class Leistungserfassung : BaseEntity
{
    public int ProjektId { get; set; }
    public int MitarbeiterId { get; set; }
    public long? JiraProjectId { get; set; }
    public DateTime Beginn { get; set; }
    public DateTime Ende { get; set; }
    public double Stunden { get; set; }
    public bool Verrechenbar { get; set; } = true;
    public string Beschreibung { get; set; }
    public string? InternBeschreibung { get; set; }
}
