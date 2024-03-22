using Ardalis.Specification;
using JiraSynchronizer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Interfaces.Repositories;

public interface IAsyncRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    IQueryable<T> QueryAll();
    Task<T> AddAsync(T entity, bool saveChanges = true);
    Task<int> SaveChangesAsync();
}
