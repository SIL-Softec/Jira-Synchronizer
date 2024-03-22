using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class ZeitklasseRepository : AsyncRepository<Zeitklasse>, IZeitklasseRepository
{
    protected readonly JiraSynchronizerContext _dbContext;
    public ZeitklasseRepository(JiraSynchronizerContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
