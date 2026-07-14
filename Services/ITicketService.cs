using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface ITicketService
{
    Task<MyTicketsPageViewModel> GetMyTicketsAsync(int customerId, string? status, string? search);
    Task<TicketQrViewModel?> GetTicketQrAsync(int ticketId, int customerId);
    string GenerateSerialNumber(int eventId, int ticketTypeId);
    string GenerateQrCodeText(Models.Ticket ticket, int eventId);
}
