using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<IEnumerable<Project>> GetProjectsByManagerAsync(int managerId);
        Task<ProjectDetailsViewModel?> GetProjectDetailsAsync(int projectId);
        Task<Project> CreateProjectAsync(ProjectFormViewModel model);
        Task<bool> UpdateProjectAsync(ProjectFormViewModel model);
        Task<bool> DeleteProjectAsync(int projectId);
    }
}
