using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(150)]
        public string ProjectName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Not Started"; // Not Started, In Progress, Completed, On Hold, Cancelled

        [Required]
        public int ManagerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(ManagerId))]
        public virtual User Manager { get; set; } = null!;

        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
