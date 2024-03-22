using System.Text;

namespace JiraSynchronizer.Core.Entities;

public class Projekt : BaseEntity
{
    public int? ProjektId { get; set; }
    public int MandantKundeId { get; set; }
    public int? StundensatzId { get; set; }
    public string Kuerzel { get; set; }
    public string? Name { get; set; }
    public decimal? Budget { get; set; }
    public bool Intern { get; set; }
    public bool Gesperrt { get; set; }
    public string? Bemerkung { get; set; }
    public int? DefaultVerrechenbar { get; set; }
    public int? BudgetKontrolle { get; set; }
    public DateTime? LastKontrolle { get; set; }
    public string? Value1 { get; set; }
    public string? Link { get; set; }
    public int? BudgetJaehrlich { get; set; }
    public decimal? BudgetFaktor { get; set; }
    public DateTime? HertragAb { get; set; }
    public string? RechnungZeile1 { get; set; }
    public string? RechnungZeile2 { get; set; }
    public string? RechnungZeile3 { get; set; }
    public string? RechnungZeile4 { get; set; }
    public string? RechnungZeile5 { get; set; }
    public string? RechnungZeile6 { get; set; }
    public string? RechnungBetrifft { get; set; }
    public string? RechnungTitel { get; set; }
    public int? RechnungNo { get; set; }
    public int? RechnungMaterial { get; set; }
    public string? RechnungZustaendig { get; set; }
    public string? RechnungBsKreis { get; set; }
    public string? RechnungBsKreisZs { get; set; }
    public decimal? Transitorisch { get; set; }
    public decimal? Projektrabatt { get; set; }
    public decimal? HertragNegPos { get; set; }
    public string? Kundenkategorie { get; set; }
    public int? BudgetLevel { get; set; }
    public string? Accounting { get; set; }
    public int? Rapporting { get; set; }
    public virtual DateTime? ErfTime { get; set; }
    public virtual string? ErfUser { get; set; }
    public virtual DateTime? MutTime { get; set; }
    public virtual string? MutUser { get; set; }
}
