using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.ViewModels;

namespace EventManagement.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    public NotificationService(INotificationRepository repo) { _repo = repo; }

    public async Task CreateAsync(int userId, string title, string message)
    {
        await _repo.AddAsync(new UserNotification
        {
            UserId = userId,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.Now
        });
    }

    public async Task<NotificationPageViewModel> GetByUserAsync(int userId)
    {
        var list = await _repo.GetByUserAsync(userId);
        var vm = new NotificationPageViewModel();
        foreach (var n in list)
        {
            vm.Items.Add(new NotificationItemViewModel
            {
                UserNotificationId = n.UserNotificationId,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
        }
        return vm;
    }

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        var n = await _repo.GetByIdAsync(notificationId);
        if (n == null || n.UserId != userId) return;
        await _repo.MarkReadAsync(n);
    }
}
