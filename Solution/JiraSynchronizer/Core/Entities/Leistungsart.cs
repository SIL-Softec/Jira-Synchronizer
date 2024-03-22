namespace JiraSynchronizer.Core.Entities;

public class Leistungsart : BaseEntity
{
    public string? Kuerzel { get; set; }
    public string Bezeichnung { get; set; }
    public string? Beschreibung { get; set; }
    public bool Gesperrt { get; set; }
    public virtual DateTime? ErfTime { get; set; }
    public virtual string? ErfUser { get; set; }
    public virtual DateTime? MutTime { get; set; }
    public virtual string? MutUser { get; set; }
}
