using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.ViewModels;

namespace EventManagement.Services.Implementations;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlists;
    private readonly IEventRepository _events;

    public WishlistService(IWishlistRepository wishlists, IEventRepository events)
    {
        _wishlists = wishlists;
        _events = events;
    }

    public async Task<(bool Ok, string? Message)> AddAsync(int eventId, int customerId)
    {
        var ev = await _events.GetByIdAsync(eventId);
        if (ev == null) return (false, "Event not found");
        if (ev.Status != "Published") return (false, "Event is not available");
        if (await _wishlists.ExistsAsync(customerId, eventId))
            return (false, "This event is already in your wishlist");

        await _wishlists.AddAsync(new Wishlist
        {
            CustomerId = customerId,
            EventId = eventId,
            CreatedAt = DateTime.Now
        });
        return (true, null);
    }

    public async Task RemoveAsync(int eventId, int customerId)
    {
        await _wishlists.RemoveAsync(customerId, eventId);
    }

    public async Task<WishlistPageViewModel> GetWishlistAsync(int customerId)
    {
        var list = await _wishlists.GetByCustomerAsync(customerId);
        var vm = new WishlistPageViewModel();
        foreach (var w in list)
        {
            if (w.Event == null) continue;
            vm.Items.Add(new WishlistItemViewModel
            {
                EventId = w.Event.EventId,
                Title = w.Event.Title,
                CategoryName = w.Event.Category?.CategoryName ?? string.Empty,
                LocationName = w.Event.Location?.LocationName ?? string.Empty,
                StartTime = w.Event.StartTime,
                Status = w.Event.Status
            });
        }
        return vm;
    }
}
