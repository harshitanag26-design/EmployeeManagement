using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagement.Models;

namespace EmployeeManagement.ViewModels
{
    public class EmployeeFormViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string? Password { get; set; } // Required only on Create

        [Required(ErrorMessage = "Please select a Role.")]
        public int RoleId { get; set; }

        public int? DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        public SelectList? Roles { get; set; }
        public SelectList? Departments { get; set; }
    }

    public class EmployeeDetailsViewModel
    {
        public User User { get; set; } = null!;
        public int TotalAssignedTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal ProductivityScore { get; set; }
        public decimal TotalEstimatedHours { get; set; }
        public decimal TotalActualHours { get; set; }
        public List<TaskItem> RecentTasks { get; set; } = new();
    }
}
