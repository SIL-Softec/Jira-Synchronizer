namespace JiraSynchronizer.Core.Entities;

public class Mitarbeiter : BaseEntity
{
    public int AdresseId { get; set; }
    public int MandantId { get; set; }
    public string Kuerzel { get; set; }
    public DateTime Eintritt { get; set; }
    public DateTime? Austritt { get; set; }
    public bool Gesperrt { get; set; }
    public string? Bemerkung { get; set; }
    public virtual DateTime? ErfTime { get; set; }
    public virtual string? ErfUser { get; set; }
    public virtual DateTime? MutTime { get; set; }
    public virtual string? MutUser { get; set; }
}
