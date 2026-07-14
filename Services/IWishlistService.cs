using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface IWishlistService
{
    Task<(bool Ok, string? Message)> AddAsync(int eventId, int customerId);
    Task RemoveAsync(int eventId, int customerId);
    Task<WishlistPageViewModel> GetWishlistAsync(int customerId);
}
