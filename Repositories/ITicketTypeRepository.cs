using EventManagement.Models;

namespace EventManagement.Repositories;

public interface ITicketTypeRepository
{
    Task<List<TicketType>> GetByEventIdAsync(int eventId);
    Task<TicketType?> GetByIdAsync(int id);
    Task<int> CountSoldAsync(int ticketTypeId);
    Task AddAsync(TicketType entity);
}
