using TaskFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Infrastructure.Persistence;

public class TaskFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(p =>
        {
            p.HasKey(x => x.Id);
            p.Property(x => x.Name).IsRequired().HasMaxLength(200);
            p.Property(x => x.Description).IsRequired().HasMaxLength(1000);
            p.Property(x => x.CreatedAt).IsRequired();
            p.Property(x => x.OwnerId).IsRequired().HasMaxLength(450);
            p.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskItem>(t =>
        {
            t.HasKey(x => x.Id);
            t.Property(x => x.Title).IsRequired().HasMaxLength(200);
            t.Property(x => x.Description).IsRequired().HasMaxLength(1000);
            t.Property(x => x.CreatedAt).IsRequired();
            t.Property(x => x.Status).IsRequired();
            t.Property(x => x.Priority).IsRequired();
            t.HasOne(x => x.Project).WithMany(p => p.Tasks).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(r =>
        {
            r.HasKey(x => x.Id);
            r.HasIndex(x => x.JwtId).IsUnique();
            r.Property(x => x.JwtId).IsRequired().HasMaxLength(64);
            r.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        });

        modelBuilder.Entity<ProjectMember>(m =>
        {
            m.HasKey(x => new { x.ProjectId, x.UserId });
            m.HasOne(x => x.Project).WithMany(p => p.Members).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            m.HasOne(x => x.User).WithMany(u => u.ProjectMemberships).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            m.Property(x => x.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<TaskAttachment>(a =>
        {
            a.HasKey(x => x.Id);
            a.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(500);
            a.Property(x => x.StoredFileName).IsRequired().HasMaxLength(100);
            a.Property(x => x.ContentType).IsRequired().HasMaxLength(200);
            a.Property(x => x.UploadedUserId).IsRequired().HasMaxLength(450);
            a.HasOne(x => x.TaskItem).WithMany(t => t.Attachments).HasForeignKey(x => x.TaskItemId).OnDelete(DeleteBehavior.Cascade);
            a.HasOne(x => x.UploadedUser).WithMany().HasForeignKey(x => x.UploadedUserId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
