using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class ProjektMitarbeiterRepository : AsyncRepository<ProjektMitarbeiter>, IProjektMitarbeiterRepository
{
    protected readonly JiraSynchronizerContext _dbContext;
    public ProjektMitarbeiterRepository(JiraSynchronizerContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
