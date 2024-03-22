using JiraSynchronizer.Core.Interfaces;

namespace JiraSynchronizer.Core.Entities;

public class Whitelist : BaseEntity
{
    public int Id { get; set; }
    public int ProjektId { get; set; }
    public string JiraProjectName { get; set; }
}
