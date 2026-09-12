using EmployeeManagement.Repositories;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public class ProductivityService : IProductivityService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;

        public ProductivityService(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        public async Task<decimal> CalculateEmployeeCompletionRateAsync(int employeeId)
        {
            var tasks = (await _taskRepository.GetTasksByAssigneeAsync(employeeId)).ToList();
            if (!tasks.Any()) return 0;

            var completed = tasks.Count(t => t.Status == "Completed");
            return Math.Round((decimal)completed / tasks.Count * 100, 1);
        }

        public async Task<decimal> CalculateEmployeeProductivityScoreAsync(int employeeId)
        {
            var tasks = (await _taskRepository.GetTasksByAssigneeAsync(employeeId)).ToList();
            if (!tasks.Any()) return 100; // No tasks assigned yet

            int total = tasks.Count;
            int completed = tasks.Count(t => t.Status == "Completed");
            int overdue = tasks.Count(t => t.IsOverdue);
            int inProgress = tasks.Count(t => t.Status == "In Progress");

            // 1. Completion component (max 60 pts)
            decimal completionScore = ((decimal)completed / total) * 60;

            // 2. Active work momentum component (max 25 pts)
            decimal progressScore = 0;
            if (inProgress > 0)
            {
                var avgProgress = tasks.Where(t => t.Status == "In Progress").Average(t => t.ProgressPercentage);
                progressScore = ((decimal)avgProgress / 100) * 25;
            }

            // 3. Efficiency component (max 15 pts)
            decimal efficiencyScore = 15;
            var completedWithHours = tasks.Where(t => t.Status == "Completed" && t.EstimatedHours > 0).ToList();
            if (completedWithHours.Any())
            {
                var totalEst = completedWithHours.Sum(t => t.EstimatedHours);
                var totalAct = completedWithHours.Sum(t => t.ActualHours);
                if (totalAct > totalEst)
                {
                    decimal penalty = Math.Min(10, ((totalAct - totalEst) / totalEst) * 10);
                    efficiencyScore -= penalty;
                }
            }

            // 4. Overdue penalty (deduct 8 pts per overdue task)
            decimal overduePenalty = overdue * 8;

            decimal totalScore = completionScore + progressScore + efficiencyScore - overduePenalty;
            if (totalScore < 0) totalScore = 0;
            if (totalScore > 100) totalScore = 100;

            return Math.Round(totalScore, 1);
        }

        public async Task<decimal> CalculateProjectProgressAsync(int projectId)
        {
            var tasks = (await _taskRepository.GetTasksByProjectAsync(projectId)).ToList();
            if (!tasks.Any()) return 0;

            var avg = tasks.Average(t => t.ProgressPercentage);
            return Math.Round((decimal)avg, 1);
        }

        public async Task<List<EmployeeReportItem>> GetEmployeeReportsAsync()
        {
            var employees = (await _userRepository.GetEmployeesAsync()).ToList();
            var reports = new List<EmployeeReportItem>();

            foreach (var emp in employees)
            {
                var tasks = (await _taskRepository.GetTasksByAssigneeAsync(emp.UserId)).ToList();
                int total = tasks.Count;
                int completed = tasks.Count(t => t.Status == "Completed");
                int pending = tasks.Count(t => t.Status == "Pending");
                int overdue = tasks.Count(t => t.IsOverdue);

                decimal rate = total > 0 ? Math.Round((decimal)completed / total * 100, 1) : 0;
                decimal score = await CalculateEmployeeProductivityScoreAsync(emp.UserId);

                reports.Add(new EmployeeReportItem
                {
                    EmployeeId = emp.UserId,
                    EmployeeName = emp.Name,
                    DepartmentName = emp.Department?.DepartmentName ?? "General",
                    AssignedTasks = total,
                    CompletedTasks = completed,
                    PendingTasks = pending,
                    OverdueTasks = overdue,
                    CompletionRate = rate,
                    TotalEstimatedHours = tasks.Sum(t => t.EstimatedHours),
                    TotalActualHours = tasks.Sum(t => t.ActualHours),
                    ProductivityScore = score
                });
            }

            return reports.OrderByDescending(r => r.ProductivityScore).ToList();
        }

        public async Task<List<ProjectReportItem>> GetProjectReportsAsync()
        {
            var projects = (await _projectRepository.GetAllWithDetailsAsync()).ToList();
            var reports = new List<ProjectReportItem>();

            foreach (var proj in projects)
            {
                var tasks = proj.Tasks.ToList();
                int total = tasks.Count;
                int completed = tasks.Count(t => t.Status == "Completed");
                int pending = tasks.Count(t => t.Status == "Pending");
                int overdue = tasks.Count(t => t.IsOverdue);
                decimal progress = total > 0 ? Math.Round((decimal)tasks.Average(t => t.ProgressPercentage), 1) : 0;

                reports.Add(new ProjectReportItem
                {
                    ProjectId = proj.ProjectId,
                    ProjectName = proj.ProjectName,
                    ManagerName = proj.Manager?.Name ?? "Unassigned",
                    Status = proj.Status,
                    StartDate = proj.StartDate,
                    EndDate = proj.EndDate,
                    TotalTasks = total,
                    CompletedTasks = completed,
                    PendingTasks = pending,
                    OverdueTasks = overdue,
                    ProgressPercentage = progress
                });
            }

            return reports.OrderByDescending(r => r.ProgressPercentage).ToList();
        }

        public async Task<MonthlyReportViewModel> GetMonthlyReportAsync(int year, int month)
        {
            var allTasks = (await _taskRepository.GetAllWithDetailsAsync()).ToList();

            var createdInMonth = allTasks.Count(t => t.CreatedAt.Year == year && t.CreatedAt.Month == month);
            var completedInMonth = allTasks.Count(t => t.CompletedDate.HasValue &&
                                                      t.CompletedDate.Value.Year == year &&
                                                      t.CompletedDate.Value.Month == month);
            var pendingInMonth = allTasks.Count(t => t.Status == "Pending" || t.Status == "In Progress");
            var overdueInMonth = allTasks.Count(t => t.IsOverdue);

            return new MonthlyReportViewModel
            {
                Year = year,
                Month = month,
                TasksCreated = createdInMonth,
                TasksCompleted = completedInMonth,
                TasksPending = pendingInMonth,
                TasksOverdue = overdueInMonth
            };
        }

        public async Task<List<EmployeeProductivitySummary>> GetTeamProductivitySummariesAsync(int managerId)
        {
            var managedProjects = (await _projectRepository.GetProjectsByManagerAsync(managerId)).ToList();
            var teamTasks = managedProjects.SelectMany(p => p.Tasks).ToList();
            var teamMemberIds = teamTasks.Select(t => t.AssignedTo).Distinct().ToList();

            var list = new List<EmployeeProductivitySummary>();
            foreach (var empId in teamMemberIds)
            {
                var emp = await _userRepository.GetUserWithDetailsAsync(empId);
                if (emp == null) continue;

                var tasks = (await _taskRepository.GetTasksByAssigneeAsync(empId)).ToList();
                int assigned = tasks.Count;
                int completed = tasks.Count(t => t.Status == "Completed");
                int overdue = tasks.Count(t => t.IsOverdue);
                decimal rate = assigned > 0 ? Math.Round((decimal)completed / assigned * 100, 1) : 0;
                decimal score = await CalculateEmployeeProductivityScoreAsync(empId);

                list.Add(new EmployeeProductivitySummary
                {
                    EmployeeId = emp.UserId,
                    EmployeeName = emp.Name,
                    DepartmentName = emp.Department?.DepartmentName ?? "General",
                    AssignedTasks = assigned,
                    CompletedTasks = completed,
                    OverdueTasks = overdue,
                    CompletionRate = rate,
                    ProductivityScore = score
                });
            }

            return list.OrderByDescending(s => s.ProductivityScore).ToList();
        }
    }
}
