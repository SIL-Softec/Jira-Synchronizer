using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Dto;

public class WorklogDto
{
    public string Email { get; set; }
    public DateTime Started { get; set; }
    public int TimeSpentSeconds { get; set; }
    public string Comment { get; set; }
    public int JiraBuchungId { get; set; }
}
