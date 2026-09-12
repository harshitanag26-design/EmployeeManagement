using EmployeeManagement.Models;
using EmployeeManagement.Repositories;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProductivityService _productivityService;

        public ProjectService(
            IProjectRepository projectRepository,
            IProductivityService productivityService)
        {
            _projectRepository = projectRepository;
            _productivityService = productivityService;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _projectRepository.GetAllWithDetailsAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Project>> GetProjectsByManagerAsync(int managerId)
        {
            return await _projectRepository.GetProjectsByManagerAsync(managerId);
        }

        public async Task<ProjectDetailsViewModel?> GetProjectDetailsAsync(int projectId)
        {
            var project = await _projectRepository.GetByIdWithDetailsAsync(projectId);
            if (project == null) return null;

            var tasks = project.Tasks.ToList();
            int total = tasks.Count;
            int completed = tasks.Count(t => t.Status == "Completed");
            int inProgress = tasks.Count(t => t.Status == "In Progress");
            int pending = tasks.Count(t => t.Status == "Pending");
            int overdue = tasks.Count(t => t.IsOverdue);
            decimal progress = total > 0 ? Math.Round((decimal)tasks.Average(t => t.ProgressPercentage), 1) : 0;

            return new ProjectDetailsViewModel
            {
                Project = project,
                TotalTasks = total,
                CompletedTasks = completed,
                InProgressTasks = inProgress,
                PendingTasks = pending,
                OverdueTasks = overdue,
                ProgressPercentage = progress,
                Tasks = tasks.OrderByDescending(t => t.DueDate).ToList()
            };
        }

        public async Task<Project> CreateProjectAsync(ProjectFormViewModel model)
        {
            var project = new Project
            {
                ProjectName = model.ProjectName,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ManagerId = model.ManagerId,
                CreatedAt = DateTime.UtcNow
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();
            return project;
        }

        public async Task<bool> UpdateProjectAsync(ProjectFormViewModel model)
        {
            var project = await _projectRepository.GetByIdAsync(model.ProjectId);
            if (project == null) return false;

            project.ProjectName = model.ProjectName;
            project.Description = model.Description;
            project.StartDate = model.StartDate;
            project.EndDate = model.EndDate;
            project.Status = model.Status;
            project.ManagerId = model.ManagerId;

            _projectRepository.Update(project);
            await _projectRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null) return false;

            _projectRepository.Remove(project);
            await _projectRepository.SaveChangesAsync();
            return true;
        }
    }
}
