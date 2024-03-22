namespace JiraSynchronizer.Core.Entities;

public class Whitelist
{
    public int Id { get; set; }
    public int ProjektId { get; set; }
    public string JiraProjectName { get; set; }
}
