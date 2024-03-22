using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class LeistungserfassungRepository : AsyncRepository<Leistungserfassung>, ILeistungserfassungRepository
{
    protected readonly JiraSynchronizerContext _dbContext;
    public LeistungserfassungRepository(JiraSynchronizerContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
