using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<IEnumerable<Project>> GetAllWithDetailsAsync();
        Task<Project?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Project>> GetProjectsByManagerAsync(int managerId);
    }
}
