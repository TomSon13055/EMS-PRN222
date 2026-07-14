using EventManagement.Repositories;
using EventManagement.ViewModels;

namespace EventManagement.Services.Implementations;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _admin;
    private readonly IUserRepository _users;
    public AdminService(IAdminRepository admin, IUserRepository users) { _admin = admin; _users = users; }

    public Task<AdminDashboardViewModel> GetDashboardAsync() => _admin.GetDashboardAsync();

    public async Task<AdminAccountsPageViewModel> GetAccountsAsync()
    {
        return new AdminAccountsPageViewModel { Accounts = await _admin.GetAccountsAsync() };
    }

    public async Task<(bool Ok, string? Error)> ToggleAccountAsync(int userId, int adminId)
    {
        if (userId == adminId) return (false, "You cannot lock your own account");
        var u = await _users.GetByIdAsync(userId);
        if (u == null) return (false, "User not found");
        u.IsActive = !u.IsActive;
        u.UpdatedAt = DateTime.Now;
        await _users.UpdateAsync(u);
        return (true, null);
    }
}
