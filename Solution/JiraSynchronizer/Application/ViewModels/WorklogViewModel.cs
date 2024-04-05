using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.ViewModels;

public class WorklogViewModel
{
    public string Email { get; set; }
    public DateTime Started { get; set; }
    public int TimeSpentSeconds { get; set; }
    public string Comment { get; set; }
    public long JiraBuchungId { get; set; }
    public string JiraProjekt { get; set; }
    public string IssueName { get; set; }
    public bool IsAuthorized { get; set; }
    public bool ExistsOnDatabase { get; set; } = false;
}
