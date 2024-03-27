using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Entities;

public class Issue : BaseEntity
{
    public string IssueName { get; set; }
    public List<Worklog> Worklogs { get; set; }
}
