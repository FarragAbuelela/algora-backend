namespace Algora.Domain.Enums;

public enum Role
{
    Student = 1,
    Mentor = 2,
    Instructor = 3,
    Monitor = 4,
    CampLeader = 5,
    CommunityLeader = 6,
    Admin = 7
}

public enum SessionType
{
    Session = 1,
    Practice = 2,
    Contest = 3,
    Review = 4,
    Workshop = 5
}

public enum TaskStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Overdue = 4,
    Cancelled = 5
}

public enum AttendanceStatus
{
    Invited = 1,
    Confirmed = 2,
    Attended = 3,
    Absent = 4
}

public enum NotificationType
{
    RoleAssigned = 1,
    TaskAssigned = 2,
    SessionCreated = 3,
    PointsAwarded = 4,
    MentorAssigned = 5,
    TeamInvitation = 6,
    AchievementEarned = 7
}

