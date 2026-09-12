namespace EmployeeManagement.ViewModels
{
    public class EmployeeReportItem
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int AssignedTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal TotalEstimatedHours { get; set; }
        public decimal TotalActualHours { get; set; }
        public decimal ProductivityScore { get; set; }
    }

    public class ProjectReportItem
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal ProgressPercentage { get; set; }
    }

    public class MonthlyReportViewModel
    {
        public int Year { get; set; } = DateTime.Today.Year;
        public int Month { get; set; } = DateTime.Today.Month;
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        public int TasksCreated { get; set; }
        public int TasksCompleted { get; set; }
        public int TasksPending { get; set; }
        public int TasksOverdue { get; set; }
        public decimal CompletionRate => (TasksCreated > 0) ? Math.Round((decimal)TasksCompleted / TasksCreated * 100, 1) : 0;
    }

    public class GlobalReportsViewModel
    {
        public List<EmployeeReportItem> EmployeeReports { get; set; } = new();
        public List<ProjectReportItem> ProjectReports { get; set; } = new();
        public MonthlyReportViewModel MonthlyReport { get; set; } = new();
    }
}
