using EventManagement.ViewModels;

namespace EventManagement.Repositories;

public interface IAdminRepository
{
    Task<AdminDashboardViewModel> GetDashboardAsync();
    Task<List<AdminAccountListItemViewModel>> GetAccountsAsync();
}
