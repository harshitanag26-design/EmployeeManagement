using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Repositories;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IProductivityService _productivityService;
        private readonly INotificationService _notificationService;

        public DashboardController(
            IUserRepository userRepository,
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IDepartmentRepository departmentRepository,
            IProductivityService productivityService,
            INotificationService notificationService)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _departmentRepository = departmentRepository;
            _productivityService = productivityService;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(Admin));
            }
            else if (User.IsInRole("Manager"))
            {
                return RedirectToAction(nameof(Manager));
            }
            else
            {
                return RedirectToAction(nameof(Employee));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var employees = (await _userRepository.GetAllWithDetailsAsync()).ToList();
            var projects = (await _projectRepository.GetAllWithDetailsAsync()).ToList();
            var tasks = (await _taskRepository.GetAllWithDetailsAsync()).ToList();
            var departments = (await _departmentRepository.GetAllWithUsersAsync()).ToList();

            var overdueTasks = tasks.Where(t => t.IsOverdue).ToList();
            var completedTasks = tasks.Count(t => t.Status == "Completed");
            var pendingTasks = tasks.Count(t => t.Status == "Pending" || t.Status == "In Progress");

            // Chart data
            var tasksByStatus = tasks
                .GroupBy(t => t.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            var tasksByPriority = tasks
                .GroupBy(t => t.Priority)
                .ToDictionary(g => g.Key, g => g.Count());

            var empByDept = departments
                .ToDictionary(d => d.DepartmentName, d => d.Users.Count);

            var projByStatus = projects
                .GroupBy(p => p.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            var vm = new AdminDashboardViewModel
            {
                TotalEmployees = employees.Count(u => u.RoleId == 3), // Count employees
                ActiveProjects = projects.Count(p => p.Status == "In Progress"),
                TotalTasks = tasks.Count,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,
                OverdueTasks = overdueTasks.Count,
                TasksByStatus = tasksByStatus,
                TasksByPriority = tasksByPriority,
                EmployeesByDepartment = empByDept,
                ProjectsByStatus = projByStatus,
                RecentTasks = tasks.Take(6).ToList(),
                OverdueTaskList = overdueTasks.Take(6).ToList()
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manager()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int managerId))
            {
                return RedirectToAction("Login", "Account");
            }

            var managedProjects = (await _projectRepository.GetProjectsByManagerAsync(managerId)).ToList();
            var teamTasks = managedProjects.SelectMany(p => p.Tasks).ToList();
            var overdueTasks = teamTasks.Where(t => t.IsOverdue).ToList();
            var completedTasks = teamTasks.Count(t => t.Status == "Completed");
            var teamProductivity = await _productivityService.GetTeamProductivitySummariesAsync(managerId);

            var vm = new ManagerDashboardViewModel
            {
                ManagerId = managerId,
                ManagerName = User.Identity?.Name ?? "Manager",
                ManagedProjectsCount = managedProjects.Count,
                TeamTasksCount = teamTasks.Count,
                CompletedTasksCount = completedTasks,
                OverdueTasksCount = overdueTasks.Count,
                ManagedProjects = managedProjects,
                TeamTasks = teamTasks.Take(8).ToList(),
                OverdueTasks = overdueTasks,
                TeamProductivity = teamProductivity
            };

            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> Employee()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int employeeId))
            {
                return RedirectToAction("Login", "Account");
            }

            var myTasks = (await _taskRepository.GetTasksByAssigneeAsync(employeeId)).ToList();
            var completed = myTasks.Count(t => t.Status == "Completed");
            var inProgress = myTasks.Count(t => t.Status == "In Progress");
            var pending = myTasks.Count(t => t.Status == "Pending");
            var overdue = myTasks.Count(t => t.IsOverdue);

            var completionRate = await _productivityService.CalculateEmployeeCompletionRateAsync(employeeId);
            var productivityScore = await _productivityService.CalculateEmployeeProductivityScoreAsync(employeeId);
            var notifications = (await _notificationService.GetUserNotificationsAsync(employeeId, 5)).ToList();

            var vm = new EmployeeDashboardViewModel
            {
                EmployeeId = employeeId,
                EmployeeName = User.Identity?.Name ?? "Employee",
                DepartmentName = User.FindFirstValue("DepartmentName") ?? "General",
                AssignedTasksCount = myTasks.Count,
                CompletedTasksCount = completed,
                InProgressTasksCount = inProgress,
                PendingTasksCount = pending,
                OverdueTasksCount = overdue,
                CompletionRate = completionRate,
                ProductivityScore = productivityScore,
                MyTasks = myTasks,
                RecentNotifications = notifications
            };

            return View(vm);
        }
    }
}
