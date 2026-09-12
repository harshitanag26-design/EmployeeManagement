using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagement.Models;

namespace EmployeeManagement.ViewModels
{
    public class TaskFormViewModel
    {
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Please select a Project.")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Task Title is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
        public string TaskTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Please assign this task to an employee.")]
        public int AssignedTo { get; set; }

        public int CreatedBy { get; set; }

        [Required]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed, On Hold, Cancelled

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Due Date is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        [Range(0, 100, ErrorMessage = "Progress percentage must be between 0 and 100.")]
        public int ProgressPercentage { get; set; } = 0;

        [Range(0, 9999, ErrorMessage = "Estimated hours must be a positive number.")]
        public decimal EstimatedHours { get; set; } = 8;

        [Range(0, 9999, ErrorMessage = "Actual hours must be a positive number.")]
        public decimal ActualHours { get; set; } = 0;

        public SelectList? Projects { get; set; }
        public SelectList? Employees { get; set; }
    }

    public class TaskDetailsViewModel
    {
        public TaskItem Task { get; set; } = null!;
        public bool CanEdit { get; set; }
        public bool CanUpdateProgress { get; set; }
        public bool IsAssignedUser { get; set; }
        public string? NewComment { get; set; }
    }

    public class TaskUpdateProgressViewModel
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "In Progress";

        [Range(0, 100, ErrorMessage = "Progress percentage must be between 0 and 100.")]
        public int ProgressPercentage { get; set; }

        [Range(0, 9999, ErrorMessage = "Actual hours must be a positive number.")]
        public decimal ActualHours { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }

    public class TaskFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public int? AssignedTo { get; set; }
        public int? ProjectId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<TaskItem> Tasks { get; set; } = new();

        public SelectList? StatusOptions { get; set; }
        public SelectList? PriorityOptions { get; set; }
        public SelectList? EmployeeOptions { get; set; }
        public SelectList? ProjectOptions { get; set; }
    }
}
