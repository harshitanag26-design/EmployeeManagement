using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        public int? DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; } = null!;

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department? Department { get; set; }

        [InverseProperty(nameof(Project.Manager))]
        public virtual ICollection<Project> ManagedProjects { get; set; } = new List<Project>();

        [InverseProperty(nameof(TaskItem.Assignee))]
        public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();

        [InverseProperty(nameof(TaskItem.Creator))]
        public virtual ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();

        public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public virtual ICollection<TaskActivityLog> ActivityLogs { get; set; } = new List<TaskActivityLog>();
    }
}
