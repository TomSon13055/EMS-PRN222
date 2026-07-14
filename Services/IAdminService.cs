using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface IAdminService
{
    Task<AdminDashboardViewModel> GetDashboardAsync();
    Task<AdminAccountsPageViewModel> GetAccountsAsync();
    Task<(bool Ok, string? Error)> ToggleAccountAsync(int userId, int adminId);
}
