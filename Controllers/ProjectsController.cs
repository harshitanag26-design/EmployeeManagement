using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagement.Repositories;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IUserRepository _userRepository;

        public ProjectsController(
            IProjectService projectService,
            IUserRepository userRepository)
        {
            _projectService = projectService;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return View(projects);
        }

        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _projectService.GetProjectDetailsAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var model = new ProjectFormViewModel
            {
                ManagerId = currentUserId,
                Managers = await GetManagersSelectListAsync(currentUserId)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectFormViewModel model)
        {
            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Project end date cannot be earlier than start date.");
            }

            if (!ModelState.IsValid)
            {
                model.Managers = await GetManagersSelectListAsync(model.ManagerId);
                return View(model);
            }

            await _projectService.CreateProjectAsync(model);
            TempData["SuccessMessage"] = $"Project '{model.ProjectName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();

            var model = new ProjectFormViewModel
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                ManagerId = project.ManagerId,
                Managers = await GetManagersSelectListAsync(project.ManagerId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectFormViewModel model)
        {
            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Project end date cannot be earlier than start date.");
            }

            if (!ModelState.IsValid)
            {
                model.Managers = await GetManagersSelectListAsync(model.ManagerId);
                return View(model);
            }

            var success = await _projectService.UpdateProjectAsync(model);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = $"Project '{model.ProjectName}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _projectService.DeleteProjectAsync(id);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = "Project deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<SelectList> GetManagersSelectListAsync(int? selectedId = null)
        {
            var managers = await _userRepository.GetManagersAsync();
            return new SelectList(managers, "UserId", "Name", selectedId);
        }
    }
}
