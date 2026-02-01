using Algora.Domain.Enums;

namespace Algora.Domain.Entities;

public class SessionAttendee
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public AttendanceStatus Status { get; set; }

    public Session Session { get; set; } = null!;
    public User User { get; set; } = null!;
}

