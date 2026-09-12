using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Data;
using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetUserWithDetailsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Include(u => u.AssignedTasks)
                .Include(u => u.ManagedProjects)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<IEnumerable<User>> GetAllWithDetailsAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetEmployeesAsync()
        {
            return await _context.Users
                .Include(u => u.Department)
                .Where(u => u.RoleId == 3 && u.IsActive) // 3 = Employee
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetManagersAsync()
        {
            return await _context.Users
                .Include(u => u.Department)
                .Where(u => (u.RoleId == 1 || u.RoleId == 2) && u.IsActive) // Admin or Manager
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
    }
}
