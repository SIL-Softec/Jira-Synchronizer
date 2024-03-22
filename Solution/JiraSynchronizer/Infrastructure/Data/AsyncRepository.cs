using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class AsyncRepository<T> : IAsyncRepository<T> where T : BaseEntity
{
    protected readonly JiraSynchronizerContext _dbContext;

    public AsyncRepository(JiraSynchronizerContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public virtual IQueryable<T> QueryAll()
    {
        return _dbContext.Set<T>();
    }

    public virtual async Task<T> AddAsync(T entity, bool saveChanges = true)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        if (saveChanges)
        {
            await this.SaveChangesAsync();
        }

        return entity;
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}
