using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagement.Repositories;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly IUserRepository _userRepository;

        public TasksController(
            ITaskService taskService,
            IProjectService projectService,
            IUserRepository userRepository)
        {
            _taskService = taskService;
            _projectService = projectService;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index(TaskFilterViewModel filter)
        {
            // If employee, limit or filter by their tasks if needed or allow filtering all tasks
            var tasks = await _taskService.GetFilteredTasksAsync(filter);
            filter.Tasks = tasks.ToList();

            await PopulateFilterDropDowns(filter);
            return View(filter);
        }

        public async Task<IActionResult> MyTasks()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var myTasks = (await _taskService.GetTasksByAssigneeAsync(currentUserId)).ToList();
            return View(myTasks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(currentUserIdStr, out int currentUserId);

            bool isAssigned = task.AssignedTo == currentUserId;
            bool isCreator = task.CreatedBy == currentUserId;
            bool isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

            var model = new TaskDetailsViewModel
            {
                Task = task,
                CanEdit = isAdminOrManager || isCreator,
                CanUpdateProgress = isAssigned || isAdminOrManager,
                IsAssignedUser = isAssigned
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(int? projectId = null)
        {
            var model = new TaskFormViewModel
            {
                ProjectId = projectId ?? 0,
                StartDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(7),
                Projects = await GetProjectsSelectListAsync(projectId),
                Employees = await GetEmployeesSelectListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskFormViewModel model)
        {
            if (model.DueDate < model.StartDate)
            {
                ModelState.AddModelError("DueDate", "Task due date cannot be earlier than start date.");
            }

            if (!ModelState.IsValid)
            {
                model.Projects = await GetProjectsSelectListAsync(model.ProjectId);
                model.Employees = await GetEmployeesSelectListAsync(model.AssignedTo);
                return View(model);
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var task = await _taskService.CreateTaskAsync(model, currentUserId);

            TempData["SuccessMessage"] = $"Task '{task.TaskTitle}' created and assigned successfully.";
            return RedirectToAction(nameof(Details), new { id = task.TaskId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            var model = new TaskFormViewModel
            {
                TaskId = task.TaskId,
                ProjectId = task.ProjectId,
                TaskTitle = task.TaskTitle,
                Description = task.Description,
                AssignedTo = task.AssignedTo,
                CreatedBy = task.CreatedBy,
                Priority = task.Priority,
                Status = task.Status,
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                ProgressPercentage = task.ProgressPercentage,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                Projects = await GetProjectsSelectListAsync(task.ProjectId),
                Employees = await GetEmployeesSelectListAsync(task.AssignedTo)
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskFormViewModel model)
        {
            if (model.DueDate < model.StartDate)
            {
                ModelState.AddModelError("DueDate", "Task due date cannot be earlier than start date.");
            }

            if (!ModelState.IsValid)
            {
                model.Projects = await GetProjectsSelectListAsync(model.ProjectId);
                model.Employees = await GetEmployeesSelectListAsync(model.AssignedTo);
                return View(model);
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _taskService.UpdateTaskAsync(model, currentUserId);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = $"Task '{model.TaskTitle}' updated successfully.";
            return RedirectToAction(nameof(Details), new { id = model.TaskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(TaskUpdateProgressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data provided for progress update.";
                return RedirectToAction(nameof(Details), new { id = model.TaskId });
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _taskService.UpdateProgressAsync(model, currentUserId);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = "Task progress updated successfully.";
            return RedirectToAction(nameof(Details), new { id = model.TaskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int taskId, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                TempData["ErrorMessage"] = "Comment text cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = taskId });
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _taskService.AddCommentAsync(taskId, currentUserId, commentText);

            TempData["SuccessMessage"] = "Comment posted.";
            return RedirectToAction(nameof(Details), new { id = taskId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _taskService.DeleteTaskAsync(id);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = "Task deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public class MoveTaskStatusRequest
        {
            public int TaskId { get; set; }
            public string NewStatus { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> MoveStatus([FromBody] MoveTaskStatusRequest request)
        {
            if (request == null || request.TaskId <= 0 || string.IsNullOrWhiteSpace(request.NewStatus))
            {
                return BadRequest(new { success = false, message = "Invalid task or status." });
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated." });
            }

            var (success, message) = await _taskService.MoveTaskStatusAsync(request.TaskId, request.NewStatus, currentUserId);
            if (!success)
            {
                return BadRequest(new { success = false, message = message });
            }

            var updatedTask = await _taskService.GetTaskByIdAsync(request.TaskId);
            return Json(new
            {
                success = true,
                taskId = updatedTask!.TaskId,
                title = updatedTask.TaskTitle,
                status = updatedTask.Status,
                progress = updatedTask.ProgressPercentage,
                completedDate = updatedTask.CompletedDate?.ToString("MMM dd"),
                isOverdue = updatedTask.IsOverdue
            });
        }

        private async Task<SelectList> GetProjectsSelectListAsync(int? selectedId = null)
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return new SelectList(projects, "ProjectId", "ProjectName", selectedId);
        }

        private async Task<SelectList> GetEmployeesSelectListAsync(int? selectedId = null)
        {
            var employees = await _userRepository.GetEmployeesAsync();
            return new SelectList(employees, "UserId", "Name", selectedId);
        }

        private async Task PopulateFilterDropDowns(TaskFilterViewModel filter)
        {
            var statuses = new List<string> { "All", "Pending", "In Progress", "Completed", "On Hold", "Cancelled" };
            filter.StatusOptions = new SelectList(statuses, filter.Status ?? "All");

            var priorities = new List<string> { "All", "Low", "Medium", "High", "Critical" };
            filter.PriorityOptions = new SelectList(priorities, filter.Priority ?? "All");

            var employees = (await _userRepository.GetEmployeesAsync()).ToList();
            filter.EmployeeOptions = new SelectList(employees, "UserId", "Name", filter.AssignedTo);

            var projects = (await _projectService.GetAllProjectsAsync()).ToList();
            filter.ProjectOptions = new SelectList(projects, "ProjectId", "ProjectName", filter.ProjectId);
        }
    }
}
