using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Data;
using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Project>> GetAllWithDetailsAsync()
        {
            return await _context.Projects
                .Include(p => p.Manager)
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Manager)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Assignee)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
        }

        public async Task<IEnumerable<Project>> GetProjectsByManagerAsync(int managerId)
        {
            return await _context.Projects
                .Include(p => p.Manager)
                .Include(p => p.Tasks)
                .Where(p => p.ManagerId == managerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
