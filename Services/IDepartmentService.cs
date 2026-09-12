using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<DepartmentViewModel?> GetDepartmentDetailsAsync(int id);
        Task<Department> CreateDepartmentAsync(DepartmentViewModel model);
        Task<bool> UpdateDepartmentAsync(DepartmentViewModel model);
        Task<bool> ToggleStatusAsync(int id);
    }
}
