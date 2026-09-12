using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, int limit = 10);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int userId);
        Task NotifyUserAsync(int userId, string title, string message);
    }
}
