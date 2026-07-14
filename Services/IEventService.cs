using EventManagement.Models;
using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface IEventService
{
    Task<EventListPageViewModel> GetEventListPageAsync(EventFilterViewModel filter, int? currentUserId, string? role);
    Task<EventDetailViewModel?> GetEventDetailAsync(int eventId);
    Task<(bool Ok, string? Error, int? Id)> CreateEventAsync(CreateEventViewModel model, int hostId);
    Task<(bool Ok, string? Error, EditEventViewModel? Model)> GetEditViewModelAsync(int eventId, int userId, string role);
    Task<(bool Ok, string? Error)> UpdateEventAsync(EditEventViewModel model, int userId, string role);
    Task<HostMyEventsPageViewModel> GetHostMyEventsAsync(int hostId, string? status, string? search);
}
