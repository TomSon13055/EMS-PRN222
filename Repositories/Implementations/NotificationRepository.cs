using EventManagement.Data;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Repositories.Implementations;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _db;
    public NotificationRepository(ApplicationDbContext db) { _db = db; }

    public async Task AddAsync(UserNotification n)
    {
        _db.UserNotifications.Add(n);
        await _db.SaveChangesAsync();
    }

    public Task<List<UserNotification>> GetByUserAsync(int userId) =>
        _db.UserNotifications.AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public Task<UserNotification?> GetByIdAsync(int id) =>
        _db.UserNotifications.FirstOrDefaultAsync(n => n.UserNotificationId == id);

    public async Task MarkReadAsync(UserNotification n)
    {
        n.IsRead = true;
        _db.UserNotifications.Update(n);
        await _db.SaveChangesAsync();
    }
}
