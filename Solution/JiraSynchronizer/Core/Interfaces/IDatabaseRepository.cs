using JiraSynchronizer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Interfaces;

public interface IDatabaseRepository<T> where T : BaseEntity
{
    List<T> ListAll();
    void AddAll(List<T> entries);
}
