using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.ViewModels;

public class IssueViewModel
{
    public string IssueName { get; set; }
    public List<WorklogViewModel> Worklogs { get; set; }
}
