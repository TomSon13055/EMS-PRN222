using EventManagement.Data;
using EventManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Repositories.Implementations;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _db;
    public AdminRepository(ApplicationDbContext db) { _db = db; }

    public async Task<AdminDashboardViewModel> GetDashboardAsync()
    {
        var vm = new AdminDashboardViewModel
        {
            TotalUsers = await _db.Users.CountAsync(),
            TotalCustomers = await _db.Users.CountAsync(u => u.Role == "Customer"),
            TotalHosts = await _db.Users.CountAsync(u => u.Role == "Host"),
            TotalEvents = await _db.Events.CountAsync(),
            TotalPublishedEvents = await _db.Events.CountAsync(e => e.Status == "Published"),
            TotalTicketsSold = await _db.Tickets.CountAsync(t => t.Status == "Assigned" || t.Status == "Used"),
            TotalRevenue = await _db.Orders.Where(o => o.Status == "Paid").SumAsync(o => (decimal?)o.FinalAmount) ?? 0m
        };
        return vm;
    }

    public async Task<List<AdminAccountListItemViewModel>> GetAccountsAsync()
    {
        return await _db.Users.AsNoTracking()
            .OrderBy(u => u.UserId)
            .Select(u => new AdminAccountListItemViewModel
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }
}
