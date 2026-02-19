using ASP_NET_19._TaskFlow_Files.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_19._TaskFlow_Files.Data;

public class TaskFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public TaskFlowDbContext(DbContextOptions options)
        : base(options)
    { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskAttachment> Attachments => Set<TaskAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Project
        modelBuilder.Entity<Project>(
            project =>
            {
                project.HasKey(p => p.Id);
                project.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                project.Property(p => p.Description)
                    .IsRequired()
                    .HasMaxLength(1000);
                project.Property(p => p.CreatedAt)
                    .IsRequired();
                project.Property(p => p.OwnerId)
                    .IsRequired()
                    .HasMaxLength(450);

                project.HasOne(p => p.Owner)
                    .WithMany()
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
            );

        // TaskItem
        modelBuilder.Entity<TaskItem>(
            task =>
            {
                task.HasKey(t => t.Id);
                task.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                task.Property(t => t.Description)
                    .IsRequired()
                    .HasMaxLength(1000);
                task.Property(t => t.CreatedAt)
                    .IsRequired();
                task.Property(t => t.Status)
                    .IsRequired();
                task.Property(t => t.Priority)
                    .IsRequired();

                task.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
            );

        // Refresh token
        modelBuilder.Entity<RefreshToken>(
            refresh =>
            {
                refresh.HasKey(rt => rt.Id);
                refresh.HasIndex(rt => rt.JwtId).IsUnique();
                refresh.Property(rt => rt.JwtId).IsRequired().HasMaxLength(64);
                refresh.Property(rt => rt.UserId).IsRequired().HasMaxLength(450);
            }
            );

        // Project Member
        modelBuilder.Entity<ProjectMember>(
            member =>
            {
                member.HasKey(m => new { m.ProjectId, m.UserId });
                member.HasOne(m => m.Project)
                    .WithMany(p => p.Members)
                    .HasForeignKey(m => m.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
                member.HasOne(m => m.User)
                    .WithMany(u=> u.ProjectMemberships)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                member.Property(m => m.UserId)
                    .HasMaxLength(450);
            }
            );

        // TaskAttachment
        modelBuilder.Entity<TaskAttachment>(
            attachment =>
            {
                attachment.HasKey(ta => ta.Id);

                attachment
                    .Property(ta => ta.OriginalFileName)
                    .IsRequired()
                    .HasMaxLength(500);

                attachment
                    .Property(ta => ta.StoredFileName)
                    .IsRequired()
                    .HasMaxLength(100);

                attachment
                    .Property(ta => ta.ContentType)
                    .IsRequired()
                    .HasMaxLength(200);

                attachment
                    .Property(ta => ta.UploadedUserId)
                    .IsRequired()
                    .HasMaxLength(450);

                attachment
                    .HasOne(ta => ta.TaskItem)
                    .WithMany(t => t.Attachments)
                    .HasForeignKey(ta => ta.TaskItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                attachment
                    .HasOne(ta => ta.UploadedUser)
                    .WithMany()
                    .HasForeignKey(ta => ta.UploadedUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
            );
    }
}
