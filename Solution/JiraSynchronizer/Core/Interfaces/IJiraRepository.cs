using JiraSynchronizer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Interfaces;

public interface IJiraRepository
{
    public Task<List<Issue>> GetIssuesAsync(string jiraProject);
    public Task<List<Worklog>> GetWorklogsAsync(string issueName);
}
