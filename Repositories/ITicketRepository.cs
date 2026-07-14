using EventManagement.Models;

namespace EventManagement.Repositories;

public interface ITicketRepository
{
    Task<List<Ticket>> GetByCustomerIdAsync(int customerId, string? status, string? search);
    Task<Ticket?> GetByIdAsync(int id);
    Task<int> CountByCustomerAsync(int customerId, string? status);
    Task AddRangeAsync(IEnumerable<Ticket> tickets);
}
