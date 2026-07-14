using EventManagement.Data;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Repositories.Implementations;

public class TicketTypeRepository : ITicketTypeRepository
{
    private readonly ApplicationDbContext _db;
    public TicketTypeRepository(ApplicationDbContext db) { _db = db; }

    public async Task<List<TicketType>> GetByEventIdAsync(int eventId)
    {
        Console.WriteLine($"[REPO] GetByEventIdAsync called with eventId={eventId}");
        var now = DateTime.UtcNow;
        var result = await _db.TicketTypes.AsNoTracking()
            .Where(t => t.EventId == eventId && t.SaleStart <= now && t.SaleEnd >= now)
            .OrderBy(t => t.TicketTypeId)
            .ToListAsync();
        Console.WriteLine($"[REPO] GetByEventIdAsync returned {result.Count} rows");
        return result;
    }

    public Task<TicketType?> GetByIdAsync(int id) =>
        _db.TicketTypes.FirstOrDefaultAsync(t => t.TicketTypeId == id);

    public Task<int> CountSoldAsync(int ticketTypeId) =>
        _db.Tickets.AsNoTracking()
            .Where(t => t.TicketTypeId == ticketTypeId && (t.Status == "Assigned" || t.Status == "Used"))
            .CountAsync();

    public async Task AddAsync(TicketType entity)
    {
        _db.TicketTypes.Add(entity);
        await _db.SaveChangesAsync();
    }
}
