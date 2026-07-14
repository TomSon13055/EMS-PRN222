using EventManagement.Models;

namespace EventManagement.Repositories;

public interface IWishlistRepository
{
    Task<bool> ExistsAsync(int customerId, int eventId);
    Task AddAsync(Wishlist wishlist);
    Task RemoveAsync(int customerId, int eventId);
    Task<List<Wishlist>> GetByCustomerAsync(int customerId);
}
