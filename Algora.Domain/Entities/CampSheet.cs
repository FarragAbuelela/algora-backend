namespace Algora.Domain.Entities;

public class CampSheet
{
    public Guid Id { get; set; }
    public Guid CampId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }

    public Camp Camp { get; set; } = null!;
}

