namespace JiraSynchronizer.Application.ViewModels;

public class LeistungserfassungViewModel
{
    public int Id { get; set; }
    public string Mitarbeiter { get; set; }
    public string LeistungsArt { get; set; }
    public string Zeitklasse { get; set; }
    public string JiraProjectName { get; set; }
    public DateTime Beginn { get; set; }
    public DateTime Ende { get; set; }
    public int Stunden { get; set; }
    public bool Verrechenbar { get; set; } = true;
    public string Beschreibung { get; set; }
    public string? InternBeschreibung { get; set; }
}
