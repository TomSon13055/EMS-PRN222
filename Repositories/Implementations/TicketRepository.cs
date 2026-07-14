using EventManagement.Data;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Repositories.Implementations;

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _db;
    public TicketRepository(ApplicationDbContext db) { _db = db; }

    public Task<List<Ticket>> GetByCustomerIdAsync(int customerId, string? status, string? search)
    {
        var q = _db.Tickets.AsNoTracking()
            .Include(t => t.TicketType)!.ThenInclude(tt => tt!.Event)
            .Include(t => t.OrderItem)!.ThenInclude(oi => oi!.Order)
            .Where(t => t.OrderItem!.Order!.CustomerId == customerId
                && t.OrderItem.Order.Status == "Paid");

        if (!string.IsNullOrEmpty(status) && status != "All")
            q = q.Where(t => t.Status == status);

        if (!string.IsNullOrEmpty(search))
        {
            var s = search.Trim();
            q = q.Where(t => t.SerialNumber.Contains(s) || (t.TicketType!.Event!.Title != null && t.TicketType.Event.Title.Contains(s)));
        }

        return q.OrderByDescending(t => t.IssuedAt).ToListAsync();
    }

    public Task<Ticket?> GetByIdAsync(int id) =>
        _db.Tickets
            .Include(t => t.TicketType)!.ThenInclude(tt => tt!.Event)!.ThenInclude(e => e!.Location)
            .Include(t => t.OrderItem)!.ThenInclude(oi => oi!.Order)
            .FirstOrDefaultAsync(t => t.TicketId == id);

    public Task<int> CountByCustomerAsync(int customerId, string? status)
    {
        var q = _db.Tickets.AsNoTracking()
            .Where(t => t.OrderItem!.Order!.CustomerId == customerId
                && t.OrderItem.Order.Status == "Paid");
        if (!string.IsNullOrEmpty(status) && status != "All")
            q = q.Where(t => t.Status == status);
        return q.CountAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Ticket> tickets)
    {
        _db.Tickets.AddRange(tickets);
        await _db.SaveChangesAsync();
    }
}
