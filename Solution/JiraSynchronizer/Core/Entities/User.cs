namespace JiraSynchronizer.Core.Entities;

public class User : BaseEntity
{
    public int MitarbeiterId { get; set; }
    public string UniqueName { get; set; }
    public string Name { get; set; }
}
