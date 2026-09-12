using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await _departmentService.GetDepartmentDetailsAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new DepartmentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _departmentService.CreateDepartmentAsync(model);
            TempData["SuccessMessage"] = $"Department '{model.DepartmentName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dept = await _departmentService.GetDepartmentByIdAsync(id);
            if (dept == null) return NotFound();

            var model = new DepartmentViewModel
            {
                DepartmentId = dept.DepartmentId,
                DepartmentName = dept.DepartmentName,
                Description = dept.Description,
                IsActive = dept.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _departmentService.UpdateDepartmentAsync(model);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = $"Department '{model.DepartmentName}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _departmentService.ToggleStatusAsync(id);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = "Department status toggled successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
