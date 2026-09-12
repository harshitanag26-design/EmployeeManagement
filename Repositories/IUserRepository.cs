using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserWithDetailsAsync(int userId);
        Task<IEnumerable<User>> GetAllWithDetailsAsync();
        Task<IEnumerable<User>> GetEmployeesAsync();
        Task<IEnumerable<User>> GetManagersAsync();
    }
}
