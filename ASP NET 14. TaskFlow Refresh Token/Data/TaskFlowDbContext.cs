using ASP_NET_14._TaskFlow_Refresh_Token.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_14._TaskFlow_Refresh_Token.Data;

public class TaskFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public TaskFlowDbContext(DbContextOptions options)
        : base(options)
    { }
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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

        modelBuilder.Entity<RefreshToken>(
            refresh =>
            {
                refresh.HasKey(rt => rt.Id);
                refresh.HasIndex(rt => rt.JwtId).IsUnique();
                refresh.Property(rt => rt.JwtId).IsRequired().HasMaxLength(64);
                refresh.Property(rt => rt.UserId).IsRequired().HasMaxLength(64);
            }
            );
    }
}
