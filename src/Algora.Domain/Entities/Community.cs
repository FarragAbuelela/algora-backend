namespace Algora.Domain.Entities;

public class Community
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Camp> Camps { get; set; } = new List<Camp>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

