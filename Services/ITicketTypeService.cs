using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface ITicketTypeService
{
    Task<(bool Ok, string? Error)> CreateTicketTypeAsync(CreateTicketTypeViewModel model, int hostId);
    Task<List<TicketTypeViewModel>> GetByEventAsync(int eventId);
}
