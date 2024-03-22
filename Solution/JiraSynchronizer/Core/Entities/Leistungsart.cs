namespace JiraSynchronizer.Core.Entities;

public class Leistungsart : BaseEntity
{
    public string? Kuerzel { get; set; }
    public string Bezeichnung { get; set; }
    public string? Beschreibung { get; set; }
    public bool Gesperrt { get; set; }
}
