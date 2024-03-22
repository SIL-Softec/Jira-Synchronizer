namespace JiraSynchronizer.Core.Entities;

public class ProjektMitarbeiter : BaseEntity
{
    public int ProjektId { get; set; }
    public int MitarbeiterId { get; set; }
    public int? StundensatzId { get; set; }
    public bool Projektleiter { get; set; }
    public string? Bemerkung { get; set; }
    public bool Gesperrt { get; set; }
    public bool? PlVerrechnung { get; set; }
}
