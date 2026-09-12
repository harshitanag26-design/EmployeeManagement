using EmployeeManagement.Models;

namespace EmployeeManagement.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int ActiveProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }

        public decimal OverallCompletionRate => TotalTasks > 0 ? Math.Round((decimal)CompletedTasks / TotalTasks * 100, 1) : 0;

        // Chart Data
        public Dictionary<string, int> TasksByStatus { get; set; } = new();
        public Dictionary<string, int> TasksByPriority { get; set; } = new();
        public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
        public Dictionary<string, int> ProjectsByStatus { get; set; } = new();

        public List<TaskItem> RecentTasks { get; set; } = new();
        public List<TaskItem> OverdueTaskList { get; set; } = new();
        public List<TaskActivityLog> RecentActivities { get; set; } = new();
    }

    public class ManagerDashboardViewModel
    {
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;

        public int ManagedProjectsCount { get; set; }
        public int TeamTasksCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public int OverdueTasksCount { get; set; }

        public decimal TeamCompletionRate => TeamTasksCount > 0 ? Math.Round((decimal)CompletedTasksCount / TeamTasksCount * 100, 1) : 0;

        public List<Project> ManagedProjects { get; set; } = new();
        public List<TaskItem> TeamTasks { get; set; } = new();
        public List<TaskItem> OverdueTasks { get; set; } = new();
        public List<EmployeeProductivitySummary> TeamProductivity { get; set; } = new();
    }

    public class EmployeeDashboardViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

        public int AssignedTasksCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public int InProgressTasksCount { get; set; }
        public int PendingTasksCount { get; set; }
        public int OverdueTasksCount { get; set; }

        public decimal CompletionRate { get; set; }
        public decimal ProductivityScore { get; set; } // Based on on-time, efficiency, completion

        public List<TaskItem> MyTasks { get; set; } = new();
        public List<TaskItem> PendingOrInProgressTasks => MyTasks.Where(t => t.Status != "Completed" && t.Status != "Cancelled").ToList();
        public List<TaskItem> OverdueTasks => MyTasks.Where(t => t.IsOverdue).ToList();
        public List<Notification> RecentNotifications { get; set; } = new();
    }

    public class EmployeeProductivitySummary
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int AssignedTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal ProductivityScore { get; set; }
    }
}
