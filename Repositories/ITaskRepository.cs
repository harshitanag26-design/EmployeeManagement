using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetAllWithDetailsAsync();
        Task<TaskItem?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(int employeeId);
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
        Task<IEnumerable<TaskItem>> GetFilteredTasksAsync(
            string? searchTerm,
            string? status,
            string? priority,
            int? assignedTo,
            int? projectId,
            DateTime? fromDate,
            DateTime? toDate);

        Task AddCommentAsync(TaskComment comment);
        Task AddActivityLogAsync(TaskActivityLog log);
    }
}
