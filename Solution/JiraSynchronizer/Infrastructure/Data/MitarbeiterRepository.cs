using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class MitarbeiterRepository : AsyncRepository<Mitarbeiter>, IMitarbeiterRepository
{
    protected readonly JiraSynchronizerContext _dbContext;
    public MitarbeiterRepository(JiraSynchronizerContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
