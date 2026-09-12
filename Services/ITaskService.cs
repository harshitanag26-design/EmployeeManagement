using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(int employeeId);
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId);
        Task<IEnumerable<TaskItem>> GetFilteredTasksAsync(TaskFilterViewModel filter);
        Task<TaskItem> CreateTaskAsync(TaskFormViewModel model, int creatorUserId);
        Task<bool> UpdateTaskAsync(TaskFormViewModel model, int currentUserId);
        Task<bool> UpdateProgressAsync(TaskUpdateProgressViewModel model, int currentUserId);
        Task<bool> DeleteTaskAsync(int taskId);
        Task<bool> AddCommentAsync(int taskId, int userId, string commentText);
        Task<(bool Success, string Message)> MoveTaskStatusAsync(int taskId, string newStatus, int currentUserId);
    }
}
