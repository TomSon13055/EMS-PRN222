using EventManagement.Models;
using EventManagement.ViewModels;

namespace EventManagement.Repositories;

public interface IEventRepository
{
    IQueryable<Event> GetEventsQuery();
    Task<Event?> GetByIdAsync(int id);
    Task<List<EventCategory>> GetCategoriesAsync();
    Task<List<EventLocation>> GetLocationsAsync();
    Task AddAsync(Event entity);
    Task UpdateAsync(Event entity);
    Task<bool> HasSoldTicketsAsync(int eventId);
    Task<int> CountSoldTicketsAsync(int eventId);
    Task<decimal> SumRevenueAsync(int eventId);
}
