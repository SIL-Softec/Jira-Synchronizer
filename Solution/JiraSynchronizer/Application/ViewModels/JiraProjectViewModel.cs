using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.ViewModels;

public class JiraProjectViewModel
{
    public string ProjectName { get; set; }
    public int LeisProjectId { get; set; }
    public List<IssueViewModel> Issues { get; set; }
}
