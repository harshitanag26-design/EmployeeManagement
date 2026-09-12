using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Data;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly ApplicationDbContext _context;

        public EmployeesController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            ApplicationDbContext context)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? departmentId, bool? status)
        {
            var employees = (await _employeeService.GetAllEmployeesAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                employees = employees.Where(e => e.Name.ToLower().Contains(s) || e.Email.ToLower().Contains(s)).ToList();
            }

            if (departmentId.HasValue && departmentId.Value > 0)
            {
                employees = employees.Where(e => e.DepartmentId == departmentId.Value).ToList();
            }

            if (status.HasValue)
            {
                employees = employees.Where(e => e.IsActive == status.Value).ToList();
            }

            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName", departmentId);
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;

            return View(employees);
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await _employeeService.GetEmployeeDetailsAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EmployeeFormViewModel
            {
                Roles = await GetRolesSelectListAsync(),
                Departments = await GetDepartmentsSelectListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required for new employees.");
            }

            if (!await _employeeService.IsEmailUniqueAsync(model.Email))
            {
                ModelState.AddModelError("Email", "An employee with this email already exists.");
            }

            if (!ModelState.IsValid)
            {
                model.Roles = await GetRolesSelectListAsync(model.RoleId);
                model.Departments = await GetDepartmentsSelectListAsync(model.DepartmentId);
                return View(model);
            }

            await _employeeService.CreateEmployeeAsync(model);
            TempData["SuccessMessage"] = $"Employee '{model.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _employeeService.GetEmployeeByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EmployeeFormViewModel
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                DepartmentId = user.DepartmentId,
                IsActive = user.IsActive,
                Roles = await GetRolesSelectListAsync(user.RoleId),
                Departments = await GetDepartmentsSelectListAsync(user.DepartmentId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeFormViewModel model)
        {
            if (!await _employeeService.IsEmailUniqueAsync(model.Email, model.UserId))
            {
                ModelState.AddModelError("Email", "Another employee is already using this email.");
            }

            if (!ModelState.IsValid)
            {
                model.Roles = await GetRolesSelectListAsync(model.RoleId);
                model.Departments = await GetDepartmentsSelectListAsync(model.DepartmentId);
                return View(model);
            }

            var success = await _employeeService.UpdateEmployeeAsync(model);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = $"Employee '{model.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _employeeService.ToggleStatusAsync(id);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = "Employee status updated.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<SelectList> GetRolesSelectListAsync(int? selectedId = null)
        {
            var roles = await _context.Roles.OrderBy(r => r.RoleId).ToListAsync();
            return new SelectList(roles, "RoleId", "RoleName", selectedId);
        }

        private async Task<SelectList> GetDepartmentsSelectListAsync(int? selectedId = null)
        {
            var departments = (await _departmentService.GetAllDepartmentsAsync()).Where(d => d.IsActive).OrderBy(d => d.DepartmentName);
            return new SelectList(departments, "DepartmentId", "DepartmentName", selectedId);
        }
    }
}
