using Algora.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Persistence;

public class AlgoraDbContext : DbContext
{
    public AlgoraDbContext(DbContextOptions<AlgoraDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Community> Communities => Set<Community>();
    public DbSet<Camp> Camps => Set<Camp>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<CampSheet> CampSheets => Set<CampSheet>();
    public DbSet<Problem> Problems => Set<Problem>();
    public DbSet<UserCampPoints> UserCampPoints => Set<UserCampPoints>();
    public DbSet<SolvedProblem> SolvedProblems => Set<SolvedProblem>();
    public DbSet<MentorAssignment> MentorAssignments => Set<MentorAssignment>();
    public DbSet<UserTask> UserTasks => Set<UserTask>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionAttendee> SessionAttendees => Set<SessionAttendee>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Discussion> Discussions => Set<Discussion>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SCNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.SCNumber).HasMaxLength(14).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
        });

        // Community
        modelBuilder.Entity<Community>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
        });

        // Camp
        modelBuilder.Entity<Camp>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Community).WithMany(c => c.Camps).HasForeignKey(e => e.CommunityId).OnDelete(DeleteBehavior.Cascade);
        });

        // UserRole
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User).WithMany(u => u.UserRoles).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Camp).WithMany(c => c.UserRoles).HasForeignKey(e => e.CampId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Community).WithMany(c => c.UserRoles).HasForeignKey(e => e.CommunityId).OnDelete(DeleteBehavior.NoAction);
        });

        // CampSheet
        modelBuilder.Entity<CampSheet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Camp).WithMany(c => c.CampSheets).HasForeignKey(e => e.CampId).OnDelete(DeleteBehavior.Cascade);
        });

        // Problem
        modelBuilder.Entity<Problem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Camp).WithMany(c => c.Problems).HasForeignKey(e => e.CampId).OnDelete(DeleteBehavior.Cascade);
        });

        // UserCampPoints
        modelBuilder.Entity<UserCampPoints>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.CampId }).IsUnique();
            entity.HasOne(e => e.User).WithMany(u => u.UserCampPoints).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Camp).WithMany(c => c.UserCampPoints).HasForeignKey(e => e.CampId).OnDelete(DeleteBehavior.Cascade);
        });

        // SolvedProblem
        modelBuilder.Entity<SolvedProblem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.ProblemId }).IsUnique();
            entity.HasOne(e => e.User).WithMany(u => u.SolvedProblems).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Problem).WithMany(p => p.SolvedProblems).HasForeignKey(e => e.ProblemId).OnDelete(DeleteBehavior.NoAction);
        });

        // MentorAssignment
        modelBuilder.Entity<MentorAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Mentor).WithMany(u => u.MentorAssignments).HasForeignKey(e => e.MentorId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Student).WithMany(u => u.StudentAssignments).HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Camp).WithMany().HasForeignKey(e => e.CampId).OnDelete(DeleteBehavior.NoAction);
        });

        // UserTask
        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedTasks).HasForeignKey(e => e.CreatedByMentorId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.AssignedTo).WithMany(u => u.AssignedTasks).HasForeignKey(e => e.AssignedToStudentId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Camp).WithMany().HasForeignKey(e => e.CampId).OnDelete(DeleteBehavior.NoAction);
        });

        // Session
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Camp).WithMany(c => c.Sessions).HasForeignKey(e => e.CampId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Instructor).WithMany().HasForeignKey(e => e.InstructorId).OnDelete(DeleteBehavior.NoAction);
        });

        // SessionAttendee
        modelBuilder.Entity<SessionAttendee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SessionId, e.UserId }).IsUnique();
            entity.HasOne(e => e.Session).WithMany(s => s.SessionAttendees).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany(u => u.SessionAttendees).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User).WithMany(u => u.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.UserId, e.IsRead, e.CreatedAt });
        });

        // Team
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Camp).WithMany(c => c.Teams).HasForeignKey(e => e.CampId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.TeamLeader).WithMany().HasForeignKey(e => e.TeamLeaderId).OnDelete(DeleteBehavior.NoAction);
        });

        // TeamMember
        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TeamId, e.UserId }).IsUnique();
            entity.HasOne(e => e.Team).WithMany(t => t.TeamMembers).HasForeignKey(e => e.TeamId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany(u => u.TeamMemberships).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);
        });

        // Discussion
        modelBuilder.Entity<Discussion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Problem).WithMany(p => p.Discussions).HasForeignKey(e => e.ProblemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany(u => u.Discussions).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.ParentDiscussion).WithMany(d => d.Replies).HasForeignKey(e => e.ParentDiscussionId).OnDelete(DeleteBehavior.NoAction);
        });

        // Achievement
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
        });

        // UserAchievement
        modelBuilder.Entity<UserAchievement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.AchievementId }).IsUnique();
            entity.HasOne(e => e.User).WithMany(u => u.UserAchievements).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Achievement).WithMany(a => a.UserAchievements).HasForeignKey(e => e.AchievementId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}




