using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface IWalletService
{
    Task<(bool Ok, string? Error)> TopUpAsync(int userId, decimal amount);
    Task<WalletPageViewModel> GetWalletPageAsync(int userId);
}
