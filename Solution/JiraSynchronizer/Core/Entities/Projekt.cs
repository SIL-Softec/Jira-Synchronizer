using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Entities;

public class Projekt : BaseEntity
{
    public int Id { get; set; }
    public bool DefaultVerrechenbar { get; set; }
}
