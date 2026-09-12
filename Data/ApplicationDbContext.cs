using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Models;

namespace EmployeeManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<TaskComment> TaskComments => Set<TaskComment>();
        public DbSet<TaskActivityLog> TaskActivityLogs => Set<TaskActivityLog>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Manager" },
                new Role { RoleId = 3, RoleName = "Employee" }
            );

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.Department)
                    .WithMany(d => d.Users)
                    .HasForeignKey(u => u.DepartmentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Department configuration
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(d => d.DepartmentName).IsUnique();
            });

            // Project configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasOne(p => p.Manager)
                    .WithMany(u => u.ManagedProjects)
                    .HasForeignKey(p => p.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TaskItem configuration
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Assignee)
                    .WithMany(u => u.AssignedTasks)
                    .HasForeignKey(t => t.AssignedTo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Creator)
                    .WithMany(u => u.CreatedTasks)
                    .HasForeignKey(t => t.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TaskComment configuration
            modelBuilder.Entity<TaskComment>(entity =>
            {
                entity.HasOne(c => c.Task)
                    .WithMany(t => t.Comments)
                    .HasForeignKey(c => c.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.TaskComments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TaskActivityLog configuration
            modelBuilder.Entity<TaskActivityLog>(entity =>
            {
                entity.HasOne(a => a.Task)
                    .WithMany(t => t.ActivityLogs)
                    .HasForeignKey(a => a.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.User)
                    .WithMany(u => u.ActivityLogs)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
