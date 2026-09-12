using System.Security.Claims;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string email, string password);
        ClaimsPrincipal CreateClaimsPrincipal(User user);
        string HashPassword(User user, string password);
    }
}
