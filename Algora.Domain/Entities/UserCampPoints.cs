namespace Algora.Domain.Entities;

public class UserCampPoints
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CampId { get; set; }
    public int TotalPoints { get; set; }
    public int SolvedProblemsCount { get; set; }
    public DateTime LastUpdated { get; set; }

    public User User { get; set; } = null!;
    public Camp Camp { get; set; } = null!;
}

