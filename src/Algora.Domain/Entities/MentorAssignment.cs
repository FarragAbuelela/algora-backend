namespace Algora.Domain.Entities;

public class MentorAssignment
{
    public Guid Id { get; set; }
    public Guid MentorId { get; set; }
    public Guid StudentId { get; set; }
    public Guid? CampId { get; set; }
    public Guid AssignedBy { get; set; }
    public DateTime AssignedAt { get; set; }
    public bool IsActive { get; set; }

    public User Mentor { get; set; } = null!;
    public User Student { get; set; } = null!;
    public Camp? Camp { get; set; }
}

