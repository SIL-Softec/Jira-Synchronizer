namespace JiraSynchronizer.Core.Entities;

public class User : BaseEntity
{
    public int MitarbeiterId { get; set; }
    public string UniqueName { get; set; }
    public string Name { get; set; }
    public virtual DateTime? ErfTime { get; set; }
    public virtual string? ErfUser { get; set; }
    public virtual DateTime? MutTime { get; set; }
    public virtual string? MutUser { get; set; }
}
