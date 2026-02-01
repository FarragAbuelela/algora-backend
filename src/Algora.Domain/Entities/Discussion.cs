namespace Algora.Domain.Entities;

public class Discussion
{
    public Guid Id { get; set; }
    public Guid ProblemId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentDiscussionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Problem Problem { get; set; } = null!;
    public User User { get; set; } = null!;
    public Discussion? ParentDiscussion { get; set; }
    public ICollection<Discussion> Replies { get; set; } = new List<Discussion>();
}

