using Algora.Domain.Enums;

namespace Algora.Domain.Entities;

public class Session
{
    public Guid Id { get; set; }
    public Guid CampId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SessionType SessionType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public Guid? InstructorId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public Camp Camp { get; set; } = null!;
    public User? Instructor { get; set; }
    public ICollection<SessionAttendee> SessionAttendees { get; set; } = new List<SessionAttendee>();
}

