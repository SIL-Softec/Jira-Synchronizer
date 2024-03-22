using JiraSynchronizer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JiraSynchronizer.Infrastructure.Data;

public partial class JiraSynchronizerContext : DbContext
{
    ILoggerFactory loggerFactory = new LoggerFactory();
    public virtual DbSet<Leistungsart> TRLeistungsarten { get; set; }
    public virtual DbSet<Leistungserfassung> TLeistungserfassungen { get; set; }
    public virtual DbSet<Mitarbeiter> TMitarbeiter { get; set; }
    public virtual DbSet<Projekt> TProjekte { get; set; }
    public virtual DbSet<ProjektMitarbeiter> TZProjektMitarbeiter { get; set; }
    public virtual DbSet<User> TUsers { get; set; }
    public virtual DbSet<Whitelist> TWhitelists { get; set; }
    public virtual DbSet<Zeitklasse> TRZeitklassen { get; set; }

    public string ConnectionString { get; } = @"Server=leis-db.softec.ch;Database=SOFTEC_LEISnv;User Id=leis-app;Password=iujhZUG765:-kuhg$kuhj;TrustServerCertificate=True";

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseLoggerFactory(loggerFactory);
        options.EnableSensitiveDataLogging();
        options.UseSqlServer(ConnectionString);
        options.LogTo(Console.WriteLine).EnableDetailedErrors();
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Entitäten werden hier auf die jeweiligen Tabellen in der Datenbank gemappt.
        // Dieser Schritt könnte potentiell übersprungen werden mit entsprechender Namensgebung der verschiedenen Entitäten, diese wären dann jedoch kaum lesbar.

        modelBuilder.Entity<Leistungsart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_LEA_ID");

            entity.ToTable("TR_LEISTUNGSART", tb =>
            {
                tb.HasTrigger("TR_D_TR_LEISTUNGSART");
                tb.HasTrigger("TR_I_TR_LEISTUNGSART");
                tb.HasTrigger("TR_U_TR_LEISTUNGSART");
            });

            entity.HasIndex(e => e.Bezeichnung, "UN_LEA_BEZEICHNUNG")
                .IsUnique()
                .HasFillFactor(90);

            entity.Property(e => e.Id).HasColumnName("LEA_ID");
            entity.Property(e => e.ErfTime)
                .HasColumnType("datetime")
                .HasColumnName("ERF_TIME");
            entity.Property(e => e.ErfUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ERF_USER");
            entity.Property(e => e.Beschreibung)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("LEA_BESCHREIBUNG");
            entity.Property(e => e.Bezeichnung)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LEA_BEZEICHNUNG");
            entity.Property(e => e.Gesperrt).HasColumnName("LEA_GESPERRT");
            entity.Property(e => e.Kuerzel)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("LEA_KZ");
            entity.Property(e => e.MutTime)
                .HasColumnType("datetime")
                .HasColumnName("MUT_TIME");
            entity.Property(e => e.MutUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MUT_USER");
        });

        modelBuilder.Entity<Leistungserfassung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_LEI_ID");

            entity.ToTable("T_LEISTUNGSERFASSUNG", tb =>
            {
                tb.HasTrigger("TR_I_T_LEISTUNGSERFASSUNG");
                tb.HasTrigger("TR_U_LEISTUNGSERFASSUNG");
                tb.HasTrigger("TR_U_T_LEISTUNGSERFASSUNG");
            });

            entity.HasIndex(e => e.LeistungsArtId, "FK_LEI_LEA_ID").HasFillFactor(90);

            entity.HasIndex(e => e.MitarbeiterId, "FK_LEI_MIT_ID").HasFillFactor(90);

            entity.HasIndex(e => e.ProjektId, "FK_LEI_PRJ_ID").HasFillFactor(90);

            entity.HasIndex(e => e.ZeitklasseId, "FK_LEI_ZKL_ID").HasFillFactor(90);

            entity.HasIndex(e => e.Beginn, "IN_LEI_BEGINN").HasFillFactor(90);

            entity.HasIndex(e => e.Ende, "IN_LEI_ENDE").HasFillFactor(90);

            entity.HasIndex(e => new { e.MitarbeiterId, e.Beginn, e.Ende }, "IN_MITDATUM");

            entity.Property(e => e.Id).HasColumnName("LEI_ID");
            entity.Property(e => e.ErfTime)
                .HasColumnType("datetime")
                .HasColumnName("ERF_TIME");
            entity.Property(e => e.ErfUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ERF_USER");
            entity.Property(e => e.Beginn)
                .HasColumnType("datetime")
                .HasColumnName("LEI_BEGINN");
            entity.Property(e => e.Beschreibung)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("LEI_BESCHREIBUNG");
            entity.Property(e => e.Ende)
                .HasColumnType("datetime")
                .HasColumnName("LEI_ENDE");
            entity.Property(e => e.Frozen)
                .HasColumnType("datetime")
                .HasColumnName("LEI_FROZEN");
            entity.Property(e => e.FrozenStundensatz)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("LEI_FROZEN_STUNDENSATZ");
            entity.Property(e => e.InternBeschreibung)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("LEI_INTERN_BESCHREIBUNG");
            entity.Property(e => e.LeistungsArtId).HasColumnName("LEI_LEA_ID");
            entity.Property(e => e.MitarbeiterSichtbar).HasColumnName("LEI_MIA_SICHTBAR");
            entity.Property(e => e.MitarbeiterId).HasColumnName("LEI_MIT_ID");
            entity.Property(e => e.ProjektId).HasColumnName("LEI_PRJ_ID");
            entity.Property(e => e.Stunden)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("LEI_STUNDEN");
            entity.Property(e => e.StundenKorrigiert)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("LEI_STUNDEN_KORRIGIERT");
            entity.Property(e => e.Verrechenbar).HasColumnName("LEI_VERRECHENBAR");
            entity.Property(e => e.ZeitklasseId).HasColumnName("LEI_ZKL_ID");
            entity.Property(e => e.MutTime)
                .HasColumnType("datetime")
                .HasColumnName("MUT_TIME");
            entity.Property(e => e.MutUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MUT_USER");
            entity.Property(e => e.Session).HasColumnName("SESSION");
        });

        modelBuilder.Entity<Mitarbeiter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MIT_ID");

            entity.ToTable("T_MITARBEITER", tb =>
            {
                tb.HasTrigger("TR_D_T_MITARBEITER");
                tb.HasTrigger("TR_I_MITARBEITER");
                tb.HasTrigger("TR_I_T_MITARBEITER");
                tb.HasTrigger("TR_U_T_MITARBEITER");
            });

            entity.HasIndex(e => e.AdresseId, "FK_MIT_ADR_ID").HasFillFactor(90);

            entity.HasIndex(e => e.MandantId, "FK_MIT_MAN_ID").HasFillFactor(90);

            entity.HasIndex(e => e.Kuerzel, "UN_MIA_KZ")
                .IsUnique()
                .HasFillFactor(90);

            entity.Property(e => e.Id).HasColumnName("MIT_ID");
            entity.Property(e => e.ErfTime)
                .HasColumnType("datetime")
                .HasColumnName("ERF_TIME");
            entity.Property(e => e.ErfUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ERF_USER");
            entity.Property(e => e.AdresseId).HasColumnName("MIT_ADR_ID");
            entity.Property(e => e.Austritt)
                .HasColumnType("datetime")
                .HasColumnName("MIT_AUSTRITT");
            entity.Property(e => e.Bemerkung)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("MIT_BEMERKUNG");
            entity.Property(e => e.Eintritt)
                .HasColumnType("datetime")
                .HasColumnName("MIT_EINTRITT");
            entity.Property(e => e.Gesperrt).HasColumnName("MIT_GESPERRT");
            entity.Property(e => e.Kuerzel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MIT_KZ");
            entity.Property(e => e.MandantId).HasColumnName("MIT_MAN_ID");
            entity.Property(e => e.MutTime)
                .HasColumnType("datetime")
                .HasColumnName("MUT_TIME");
            entity.Property(e => e.MutUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MUT_USER");
        });

        modelBuilder.Entity<Projekt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PRJ_ID");

            entity.ToTable("T_PROJEKT", tb =>
            {
                tb.HasTrigger("TR_D_PROJEKT");
                tb.HasTrigger("TR_D_T_PROJEKT");
                tb.HasTrigger("TR_I_T_PROJEKT");
                tb.HasTrigger("TR_U_PROJEKT");
                tb.HasTrigger("TR_U_T_PROJEKT");
                tb.HasTrigger("TU_T_PROJEKT_AUDIT");
            });

            entity.HasIndex(e => e.ProjektId, "FK_PRJ_PRJ_ID").HasFillFactor(90);

            entity.HasIndex(e => e.MandantKundeId, "FK_PRJ_MAK_ID").HasFillFactor(90);

            entity.HasIndex(e => e.StundensatzId, "FK_PRJ_STS_ID").HasFillFactor(90);

            entity.HasIndex(e => new { e.ProjektId, e.MandantKundeId, e.Kuerzel }, "UN_PRJ_PRJ_MAK_KZ")
                .IsUnique()
                .HasFillFactor(90);

            entity.Property(e => e.Id).HasColumnName("PRJ_ID");
            entity.Property(e => e.ErfTime)
                .HasColumnType("datetime")
                .HasColumnName("ERF_TIME");
            entity.Property(e => e.ErfUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ERF_USER");
            entity.Property(e => e.MutTime)
                .HasColumnType("datetime")
                .HasColumnName("MUT_TIME");
            entity.Property(e => e.MutUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MUT_USER");
            entity.Property(e => e.Accounting)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PRJ_ACCOUNTING");
            entity.Property(e => e.Bemerkung)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PRJ_BEMERKUNG");
            entity.Property(e => e.Budget)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("PRJ_BUDGET");
            entity.Property(e => e.BudgetJaehrlich).HasColumnName("PRJ_BUDGET_JAEHRLICH");
            entity.Property(e => e.BudgetFaktor)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("PRJ_BUDGETFAKTOR");
            entity.Property(e => e.BudgetKontrolle).HasColumnName("PRJ_BUDGETKONTROLLE");
            entity.Property(e => e.BudgetLevel).HasColumnName("PRJ_BUDGETLEVEL");
            entity.Property(e => e.DefaultVerrechenbar).HasColumnName("PRJ_DEFAULTVERRECHENBAR");
            entity.Property(e => e.Gesperrt).HasColumnName("PRJ_GESPERRT");
            entity.Property(e => e.HertragAb)
                .HasColumnType("datetime")
                .HasColumnName("PRJ_HERTRAG_AB");
            entity.Property(e => e.HertragNegPos)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PRJ_HERTRAG_NEGPOS");
            entity.Property(e => e.Intern).HasColumnName("PRJ_INTERN");
            entity.Property(e => e.Kundenkategorie)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PRJ_KUNDENKATEGORIE");
            entity.Property(e => e.Kuerzel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PRJ_KZ");
            entity.Property(e => e.LastKontrolle)
                .HasColumnType("datetime")
                .HasColumnName("PRJ_LASTKONTROLE");
            entity.Property(e => e.Link)
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasColumnName("PRJ_LINK");
            entity.Property(e => e.MandantKundeId).HasColumnName("PRJ_MAK_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_NAME");
            entity.Property(e => e.ProjektId).HasColumnName("PRJ_PRJ_ID");
            entity.Property(e => e.Projektrabatt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PRJ_PROJEKTRABATT");
            entity.Property(e => e.Rapporting).HasColumnName("PRJ_RAPPORTIERUNG");
            entity.Property(e => e.RechnungBetrifft)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_BETRIFFT");
            entity.Property(e => e.RechnungBsKreisZs)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_BKREIS_ZS");
            entity.Property(e => e.RechnungBsKreis)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_BSKREIS");
            entity.Property(e => e.RechnungMaterial).HasColumnName("PRJ_RNG_MATERIAL_SPLITTED");
            entity.Property(e => e.RechnungNo).HasColumnName("PRJ_RNG_NO_AUTOMATIC");
            entity.Property(e => e.RechnungTitel)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_PROJEKTTITEL");
            entity.Property(e => e.RechnungZeile1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_ZEILE1");
            entity.Property(e => e.RechnungZeile2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_ZEILE2");
            entity.Property(e => e.RechnungZeile3)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_ZEILE3");
            entity.Property(e => e.RechnungZeile4)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_ZEILE4");
            entity.Property(e => e.RechnungZeile5)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_ZEILE5");
            entity.Property(e => e.RechnungZeile6)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_ZEILE6");
            entity.Property(e => e.RechnungZustaendig)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("PRJ_RNG_ZUSTAENDIG");
            entity.Property(e => e.StundensatzId).HasColumnName("PRJ_STS_ID");
            entity.Property(e => e.Transitorisch)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("prj_transitorisch");
            entity.Property(e => e.Value1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PRJ_VALUE1");
        });

        modelBuilder.Entity<ProjektMitarbeiter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PMA_ID");

            entity.ToTable("TZ_PROJEKT_MITARBEITER", tb =>
            {
                tb.HasTrigger("TR_D_PROJEKT_MITARBEITER");
                tb.HasTrigger("TR_D_TZ_PROJEKT_MITARBEITER");
                tb.HasTrigger("TR_I_TZ_PROJEKT_MITARBEITER");
                tb.HasTrigger("TR_U_PROJEKT_MITARBEITER");
                tb.HasTrigger("TR_U_TZ_PROJEKT_MITARBEITER");
            });

            entity.HasIndex(e => e.MitarbeiterId, "FK_PMA_MIT_ID").HasFillFactor(90);

            entity.HasIndex(e => e.ProjektId, "FK_PMA_PRJ_ID").HasFillFactor(90);

            entity.HasIndex(e => e.StundensatzId, "FK_PMA_STS_ID").HasFillFactor(90);

            entity.HasIndex(e => new { e.ProjektId, e.MitarbeiterId }, "UN_PMA_PRJMIT")
                .IsUnique()
                .HasFillFactor(90);

            entity.Property(e => e.Id).HasColumnName("PMA_ID");
            entity.Property(e => e.ErfTime)
                .HasColumnType("datetime")
                .HasColumnName("ERF_TIME");
            entity.Property(e => e.ErfUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ERF_USER");
            entity.Property(e => e.MutTime)
                .HasColumnType("datetime")
                .HasColumnName("MUT_TIME");
            entity.Property(e => e.MutUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MUT_USER");
            entity.Property(e => e.Bemerkung)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PMA_BEMERKUNG");
            entity.Property(e => e.Gesperrt).HasColumnName("PMA_GESPERRT");
            entity.Property(e => e.MitarbeiterId).HasColumnName("PMA_MIT_ID");
            entity.Property(e => e.PlVerrechnung).HasColumnName("PMA_PL_VERRECHNUNG");
            entity.Property(e => e.ProjektId).HasColumnName("PMA_PRJ_ID");
            entity.Property(e => e.Projektleiter).HasColumnName("PMA_PROJEKTLEITER");
            entity.Property(e => e.StundensatzId).HasColumnName("PMA_STS_ID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("USR_ID");

            entity.ToTable("T_USER");

            entity.Property(e => e.Id).HasColumnName("USR_ID");
            entity.Property(e => e.UniqueName)
                .HasMaxLength(200)
                .HasColumnName("USR_UNIQUE_NAME");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("USR_NAME");
            entity.Property(e => e.ErfTime)
                .HasColumnType("datetime")
                .HasColumnName("ERF_TIME");
            entity.Property(e => e.ErfUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ERF_USER");
            entity.Property(e => e.MutTime)
                .HasColumnType("datetime")
                .HasColumnName("MUT_TIME");
            entity.Property(e => e.MutUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MUT_USER");
        });

        modelBuilder.Entity<Whitelist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("WHL_ID");

            entity.ToTable("T_WHITELIST");

            entity.Property(e => e.Id).HasColumnName("WHL_ID");
            entity.Property(e => e.ProjektId)
                .HasColumnName("WHL_PRJ_ID");
            entity.Property(e => e.JiraProjectName)
                .HasColumnName("WHL_JIRA_PROJECT_NAME");
        });

        modelBuilder.Entity<Zeitklasse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ZKL_ID");

            entity.ToTable("TR_ZEITKLASSE", tb =>
            {
                tb.HasTrigger("TR_D_TR_ZEITKLASSE");
                tb.HasTrigger("TR_I_TR_ZEITKLASSE");
                tb.HasTrigger("TR_U_TR_ZEITKLASSE");
                tb.HasTrigger("TR_U_ZEITKLASSE");
            });

            entity.HasIndex(e => e.Bezeichnung, "UN_ZKL_BEZEICHNUNG")
                .IsUnique()
                .HasFillFactor(90);

            entity.Property(e => e.Id).HasColumnName("ZKL_ID");
            entity.Property(e => e.ErfTime)
                .HasColumnType("datetime")
                .HasColumnName("ERF_TIME");
            entity.Property(e => e.ErfUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ERF_USER");
            entity.Property(e => e.MutTime)
                .HasColumnType("datetime")
                .HasColumnName("MUT_TIME");
            entity.Property(e => e.MutUser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MUT_USER");
            entity.Property(e => e.Ansatz)
                .HasColumnType("decimal(3, 0)")
                .HasColumnName("ZKL_ANSATZ");
            entity.Property(e => e.Beschreibung)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ZKL_BESCHREIBUNG");
            entity.Property(e => e.Bezeichnung)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ZKL_BEZEICHNUNG");
            entity.Property(e => e.Gesperrt).HasColumnName("ZKL_GESPERRT");
            entity.Property(e => e.Kuerzel)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("ZKL_KZ");
        });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}