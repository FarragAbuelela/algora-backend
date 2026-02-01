namespace Algora.Domain.Entities;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CampId { get; set; }
    public Guid TeamLeaderId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Camp Camp { get; set; } = null!;
    public User TeamLeader { get; set; } = null!;
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}

