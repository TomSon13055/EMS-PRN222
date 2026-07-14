using EventManagement.Models;

namespace EventManagement.Repositories;

public interface INotificationRepository
{
    Task AddAsync(UserNotification n);
    Task<List<UserNotification>> GetByUserAsync(int userId);
    Task<UserNotification?> GetByIdAsync(int id);
    Task MarkReadAsync(UserNotification n);
}
