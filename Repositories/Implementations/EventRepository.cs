using EventManagement.Data;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Repositories.Implementations;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _db;
    public EventRepository(ApplicationDbContext db) { _db = db; }

    public IQueryable<Event> GetEventsQuery() =>
        _db.Events.AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Location)
            .Include(e => e.Host);

    public Task<Event?> GetByIdAsync(int id) =>
        _db.Events.Include(e => e.Category).Include(e => e.Location).Include(e => e.Host)
            .FirstOrDefaultAsync(e => e.EventId == id);

    public Task<List<EventCategory>> GetCategoriesAsync() =>
        _db.EventCategories.AsNoTracking().OrderBy(c => c.EventCategoryId).ToListAsync();

    public Task<List<EventLocation>> GetLocationsAsync() =>
        _db.EventLocations.AsNoTracking().OrderBy(l => l.EventLocationId).ToListAsync();

    public async Task AddAsync(Event entity)
    {
        _db.Events.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Event entity)
    {
        _db.Events.Update(entity);
        await _db.SaveChangesAsync();
    }

    public Task<bool> HasSoldTicketsAsync(int eventId) =>
        _db.Tickets.AnyAsync(t => t.OrderItem!.Order!.Status == "Paid"
            && t.TicketType!.EventId == eventId);

    public Task<int> CountSoldTicketsAsync(int eventId) =>
        _db.Tickets.AsNoTracking()
            .Where(t => t.TicketType!.EventId == eventId
                && (t.Status == "Assigned" || t.Status == "Used"))
            .CountAsync();

    public async Task<decimal> SumRevenueAsync(int eventId)
    {
        var sum = await _db.OrderItems.AsNoTracking()
            .Where(oi => oi.TicketType!.EventId == eventId && oi.Order!.Status == "Paid")
            .SumAsync(oi => (decimal?)oi.SubTotal);
        return sum ?? 0m;
    }
}
