namespace JiraSynchronizer.Core.Entities;

public class Leistungserfassung : BaseEntity
{
    public int ProjektId { get; set; }
    public int MitarbeiterId { get; set; }
    public int LeistungsArtId { get; set; }
    public int ZeitklasseId { get; set; }
    public string JiraBuchungId { get; set; }
    public DateTime Beginn { get; set; }
    public DateTime Ende { get; set; }
    public decimal? Stunden { get; set; }
    public int Verrechenbar { get; set; }
    public string? Beschreibung { get; set; }
    public string? InternBeschreibung { get; set; }
    public bool MitarbeiterSichtbar { get; set; }
    public DateTime? Frozen { get; set; }
    public decimal? FrozenStundensatz { get; set; }
    public int? Session { get; set; }
    public decimal? StundenKorrigiert { get; set; }
}
