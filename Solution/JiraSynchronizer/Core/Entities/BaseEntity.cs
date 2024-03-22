using JiraSynchronizer.Core.Interfaces.Repositories;

namespace JiraSynchronizer.Core.Entities;

public class BaseEntity : IBaseEntity
{
    public virtual int Id { get; set; }
}

