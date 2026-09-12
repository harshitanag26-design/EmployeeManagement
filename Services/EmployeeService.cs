using EmployeeManagement.Models;
using EmployeeManagement.Repositories;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProductivityService _productivityService;
        private readonly IAuthService _authService;

        public EmployeeService(
            IUserRepository userRepository,
            ITaskRepository taskRepository,
            IProductivityService productivityService,
            IAuthService authService)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _productivityService = productivityService;
            _authService = authService;
        }

        public async Task<IEnumerable<User>> GetAllEmployeesAsync()
        {
            return await _userRepository.GetAllWithDetailsAsync();
        }

        public async Task<User?> GetEmployeeByIdAsync(int id)
        {
            return await _userRepository.GetUserWithDetailsAsync(id);
        }

        public async Task<EmployeeDetailsViewModel?> GetEmployeeDetailsAsync(int id)
        {
            var user = await _userRepository.GetUserWithDetailsAsync(id);
            if (user == null) return null;

            var tasks = (await _taskRepository.GetTasksByAssigneeAsync(id)).ToList();
            int total = tasks.Count;
            int completed = tasks.Count(t => t.Status == "Completed");
            int pending = tasks.Count(t => t.Status == "Pending" || t.Status == "In Progress");
            int overdue = tasks.Count(t => t.IsOverdue);

            decimal rate = total > 0 ? Math.Round((decimal)completed / total * 100, 1) : 0;
            decimal score = await _productivityService.CalculateEmployeeProductivityScoreAsync(id);

            return new EmployeeDetailsViewModel
            {
                User = user,
                TotalAssignedTasks = total,
                CompletedTasks = completed,
                PendingTasks = pending,
                OverdueTasks = overdue,
                CompletionRate = rate,
                ProductivityScore = score,
                TotalEstimatedHours = tasks.Sum(t => t.EstimatedHours),
                TotalActualHours = tasks.Sum(t => t.ActualHours),
                RecentTasks = tasks.Take(10).ToList()
            };
        }

        public async Task<User> CreateEmployeeAsync(EmployeeFormViewModel model)
        {
            var user = new User
            {
                Name = model.Name,
                Email = model.Email.Trim().ToLower(),
                RoleId = model.RoleId,
                DepartmentId = model.DepartmentId,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var password = string.IsNullOrWhiteSpace(model.Password) ? "Employee@123" : model.Password;
            user.PasswordHash = _authService.HashPassword(user, password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeFormViewModel model)
        {
            var user = await _userRepository.GetByIdAsync(model.UserId);
            if (user == null) return false;

            user.Name = model.Name;
            user.Email = model.Email.Trim().ToLower();
            user.RoleId = model.RoleId;
            user.DepartmentId = model.DepartmentId;
            user.IsActive = model.IsActive;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.PasswordHash = _authService.HashPassword(user, model.Password);
            }

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null)
        {
            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing == null) return true;

            if (excludeUserId.HasValue && existing.UserId == excludeUserId.Value)
            {
                return true;
            }

            return false;
        }
    }
}
