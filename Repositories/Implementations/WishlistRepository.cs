using EventManagement.Data;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Repositories.Implementations;

public class WishlistRepository : IWishlistRepository
{
    private readonly ApplicationDbContext _db;
    public WishlistRepository(ApplicationDbContext db) { _db = db; }

    public Task<bool> ExistsAsync(int customerId, int eventId) =>
        _db.Wishlists.AnyAsync(w => w.CustomerId == customerId && w.EventId == eventId);

    public async Task AddAsync(Wishlist wishlist)
    {
        _db.Wishlists.Add(wishlist);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(int customerId, int eventId)
    {
        var w = await _db.Wishlists.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.EventId == eventId);
        if (w != null)
        {
            _db.Wishlists.Remove(w);
            await _db.SaveChangesAsync();
        }
    }

    public Task<List<Wishlist>> GetByCustomerAsync(int customerId) =>
        _db.Wishlists.AsNoTracking()
            .Include(w => w.Event)!.ThenInclude(e => e!.Category)
            .Include(w => w.Event)!.ThenInclude(e => e!.Location)
            .Include(w => w.Event)!.ThenInclude(e => e!.Host)
            .Where(w => w.CustomerId == customerId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
}
