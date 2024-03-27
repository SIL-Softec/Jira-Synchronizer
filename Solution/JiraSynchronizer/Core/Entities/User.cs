using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Entities;

public class User : BaseEntity
{
    public int MitarbeiterId { get; set; }
    public string UniqueName { get; set; }
}
