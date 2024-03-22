namespace JiraSynchronizer.Core.Entities;

public class BaseEntity
{
    public virtual int Id { get; set; }
    public virtual DateTime? ErfTime { get; set; }
    public virtual string? ErfUser { get; set; }
    public virtual DateTime? MutTime { get; set; }
    public virtual string? MutUser{ get; set; }
}

