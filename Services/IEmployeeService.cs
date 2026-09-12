using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<User>> GetAllEmployeesAsync();
        Task<User?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDetailsViewModel?> GetEmployeeDetailsAsync(int id);
        Task<User> CreateEmployeeAsync(EmployeeFormViewModel model);
        Task<bool> UpdateEmployeeAsync(EmployeeFormViewModel model);
        Task<bool> ToggleStatusAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null);
    }
}
