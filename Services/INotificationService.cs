using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface INotificationService
{
    Task CreateAsync(int userId, string title, string message);
    Task<NotificationPageViewModel> GetByUserAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
}
