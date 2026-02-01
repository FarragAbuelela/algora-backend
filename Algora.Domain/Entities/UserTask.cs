using Algora.Domain.Enums;

namespace Algora.Domain.Entities;

public class UserTask
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CreatedByMentorId { get; set; }
    public Guid AssignedToStudentId { get; set; }
    public Guid? CampId { get; set; }
    public DateTime? DueDate { get; set; }
    public int RequiredCount { get; set; }
    public Enums.TaskStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public User CreatedBy { get; set; } = null!;
    public User AssignedTo { get; set; } = null!;
    public Camp? Camp { get; set; }
}

