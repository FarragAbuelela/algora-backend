using Algora.Domain.Enums;

namespace Algora.Domain.Entities;

public class UserRole
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Role Role { get; set; }
    public Guid? CampId { get; set; }
    public Guid? CommunityId { get; set; }
    public DateTime AssignedAt { get; set; }

    public User User { get; set; } = null!;
    public Camp? Camp { get; set; }
    public Community? Community { get; set; }
}

