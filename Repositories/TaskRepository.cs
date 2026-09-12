using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Data;
using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public class TaskRepository : Repository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskItem>> GetAllWithDetailsAsync()
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Include(t => t.Creator)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Include(t => t.Creator)
                .Include(t => t.Comments.OrderByDescending(c => c.CreatedAt))
                    .ThenInclude(c => c.User)
                .Include(t => t.ActivityLogs.OrderByDescending(a => a.CreatedAt))
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.TaskId == id);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(int employeeId)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Include(t => t.Creator)
                .Where(t => t.AssignedTo == employeeId)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId)
        {
            return await _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.Creator)
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
        {
            var today = DateTime.Today;
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Where(t => t.Status != "Completed" && t.Status != "Cancelled" && t.DueDate < today)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetFilteredTasksAsync(
            string? searchTerm,
            string? status,
            string? priority,
            int? assignedTo,
            int? projectId,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Include(t => t.Creator)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(t => t.TaskTitle.ToLower().Contains(term) ||
                                         (t.Description != null && t.Description.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(t => t.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(priority) && priority != "All")
            {
                query = query.Where(t => t.Priority == priority);
            }

            if (assignedTo.HasValue && assignedTo.Value > 0)
            {
                query = query.Where(t => t.AssignedTo == assignedTo.Value);
            }

            if (projectId.HasValue && projectId.Value > 0)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.DueDate >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.DueDate <= toDate.Value.Date);
            }

            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task AddCommentAsync(TaskComment comment)
        {
            await _context.TaskComments.AddAsync(comment);
        }

        public async Task AddActivityLogAsync(TaskActivityLog log)
        {
            await _context.TaskActivityLogs.AddAsync(log);
        }
    }
}
