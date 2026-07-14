using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface IOrderService
{
    Task<(bool Ok, string? Error, BuyTicketViewModel? Model)> ApplyVoucherAsync(BuyTicketViewModel model, int customerId);
    Task<(bool Ok, string? Error)> BuyTicketAsync(BuyTicketViewModel model, int customerId);
    Task<BuyTicketViewModel> BuildBuyViewModelAsync(int eventId);
    Task<BuyTicketViewModel> BuildBuyViewModelAsync(int eventId, int? ticketTypeId, int quantity);
}
