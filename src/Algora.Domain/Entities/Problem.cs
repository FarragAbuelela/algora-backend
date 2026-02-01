namespace Algora.Domain.Entities;

public class Problem
{
    public Guid Id { get; set; }
    public Guid CampId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; }
    public string? Difficulty { get; set; }
    public string? Url { get; set; }
    public DateTime CreatedAt { get; set; }

    public Camp Camp { get; set; } = null!;
    public ICollection<SolvedProblem> SolvedProblems { get; set; } = new List<SolvedProblem>();
    public ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();
}

