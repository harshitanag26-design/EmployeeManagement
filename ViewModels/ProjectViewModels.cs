using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagement.Models;

namespace EmployeeManagement.ViewModels
{
    public class ProjectFormViewModel
    {
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Project Name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Project name must be between 2 and 150 characters.")]
        public string ProjectName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "In Progress"; // Not Started, In Progress, Completed, On Hold, Cancelled

        [Required(ErrorMessage = "Please select a Project Manager.")]
        public int ManagerId { get; set; }

        public SelectList? Managers { get; set; }
    }

    public class ProjectDetailsViewModel
    {
        public Project Project { get; set; } = null!;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal ProgressPercentage { get; set; }
        public List<TaskItem> Tasks { get; set; } = new();
    }
}
