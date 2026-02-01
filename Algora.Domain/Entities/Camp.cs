namespace Algora.Domain.Entities;

public class Camp
{
    public Guid Id { get; set; }
    public Guid CommunityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Community Community { get; set; } = null!;
    public ICollection<CampSheet> CampSheets { get; set; } = new List<CampSheet>();
    public ICollection<Problem> Problems { get; set; } = new List<Problem>();
    public ICollection<UserCampPoints> UserCampPoints { get; set; } = new List<UserCampPoints>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<Team> Teams { get; set; } = new List<Team>();
}

