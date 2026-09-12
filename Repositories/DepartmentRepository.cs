using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Data;
using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Department>> GetAllWithUsersAsync()
        {
            return await _context.Departments
                .Include(d => d.Users)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdWithUsersAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }
    }
}
