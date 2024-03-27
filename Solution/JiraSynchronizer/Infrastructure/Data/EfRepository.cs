using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Infrastructure.Data;

public class EfRepository<T> : IDatabaseRepository<T> where T : BaseEntity
{
    internal readonly SqlConnection Connection;
    public EfRepository(SqlConnection connection)
    {
        Connection = connection;
    }

    public List<T> ListAll()
    {
        throw new NotImplementedException();
    }

    public void AddAll(List<T> entries)
    {
        throw new NotImplementedException();
    }
}
