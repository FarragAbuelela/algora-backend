namespace Algora.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string SCNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserCampPoints> UserCampPoints { get; set; } = new List<UserCampPoints>();
    public ICollection<SolvedProblem> SolvedProblems { get; set; } = new List<SolvedProblem>();
    public ICollection<MentorAssignment> MentorAssignments { get; set; } = new List<MentorAssignment>();
    public ICollection<MentorAssignment> StudentAssignments { get; set; } = new List<MentorAssignment>();
    public ICollection<UserTask> CreatedTasks { get; set; } = new List<UserTask>();
    public ICollection<UserTask> AssignedTasks { get; set; } = new List<UserTask>();
    public ICollection<SessionAttendee> SessionAttendees { get; set; } = new List<SessionAttendee>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();
    public ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();
    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}

