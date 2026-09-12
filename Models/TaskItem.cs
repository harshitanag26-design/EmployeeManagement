using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(200)]
        public string TaskTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int AssignedTo { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        [MaxLength(30)]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed, On Hold, Cancelled

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompletedDate { get; set; }

        [Range(0, 100)]
        public int ProgressPercentage { get; set; } = 0;

        [Column(TypeName = "decimal(8, 2)")]
        public decimal EstimatedHours { get; set; } = 0;

        [Column(TypeName = "decimal(8, 2)")]
        public decimal ActualHours { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; } = null!;

        [ForeignKey(nameof(AssignedTo))]
        public virtual User Assignee { get; set; } = null!;

        [ForeignKey(nameof(CreatedBy))]
        public virtual User Creator { get; set; } = null!;

        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();

        public virtual ICollection<TaskActivityLog> ActivityLogs { get; set; } = new List<TaskActivityLog>();

        [NotMapped]
        public bool IsOverdue => Status != "Completed" && Status != "Cancelled" && DueDate.Date < DateTime.Today;
    }
}
