using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<IEnumerable<Department>> GetAllWithUsersAsync();
        Task<Department?> GetByIdWithUsersAsync(int id);
    }
}
