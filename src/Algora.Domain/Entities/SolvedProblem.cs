namespace Algora.Domain.Entities;

public class SolvedProblem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProblemId { get; set; }
    public DateTime SolvedAt { get; set; }

    public User User { get; set; } = null!;
    public Problem Problem { get; set; } = null!;
}

