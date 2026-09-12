using EmployeeManagement.Models;
using EmployeeManagement.Repositories;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllWithUsersAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _departmentRepository.GetByIdWithUsersAsync(id);
        }

        public async Task<DepartmentViewModel?> GetDepartmentDetailsAsync(int id)
        {
            var dept = await _departmentRepository.GetByIdWithUsersAsync(id);
            if (dept == null) return null;

            return new DepartmentViewModel
            {
                DepartmentId = dept.DepartmentId,
                DepartmentName = dept.DepartmentName,
                Description = dept.Description,
                IsActive = dept.IsActive,
                EmployeeCount = dept.Users.Count,
                Employees = dept.Users.OrderBy(u => u.Name).ToList()
            };
        }

        public async Task<Department> CreateDepartmentAsync(DepartmentViewModel model)
        {
            var dept = new Department
            {
                DepartmentName = model.DepartmentName,
                Description = model.Description,
                IsActive = model.IsActive
            };

            await _departmentRepository.AddAsync(dept);
            await _departmentRepository.SaveChangesAsync();
            return dept;
        }

        public async Task<bool> UpdateDepartmentAsync(DepartmentViewModel model)
        {
            var dept = await _departmentRepository.GetByIdAsync(model.DepartmentId);
            if (dept == null) return false;

            dept.DepartmentName = model.DepartmentName;
            dept.Description = model.Description;
            dept.IsActive = model.IsActive;

            _departmentRepository.Update(dept);
            await _departmentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var dept = await _departmentRepository.GetByIdAsync(id);
            if (dept == null) return false;

            dept.IsActive = !dept.IsActive;
            _departmentRepository.Update(dept);
            await _departmentRepository.SaveChangesAsync();
            return true;
        }
    }
}
