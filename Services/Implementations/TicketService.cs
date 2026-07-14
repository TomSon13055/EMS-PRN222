using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManagement.Services.Implementations;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _tickets;
    public TicketService(ITicketRepository tickets) { _tickets = tickets; }

    public async Task<MyTicketsPageViewModel> GetMyTicketsAsync(int customerId, string? status, string? search)
    {
        var list = await _tickets.GetByCustomerIdAsync(customerId, status, search);
        var vm = new MyTicketsPageViewModel
        {
            Status = status,
            Search = search,
            Statuses = new List<SelectListItem>
            {
                new() { Value = "All", Text = "All" },
                new() { Value = "Assigned", Text = "Assigned" },
                new() { Value = "Used", Text = "Used" },
                new() { Value = "Refunded", Text = "Refunded" }
            }
        };
        foreach (var t in list)
        {
            var actualPrice = t.TicketType?.Price ?? 0m;
            if (t.OrderItem != null && t.OrderItem.Quantity > 0 && t.OrderItem.Order != null)
            {
                actualPrice = t.OrderItem.Order.FinalAmount / t.OrderItem.Quantity;
            }
            vm.Tickets.Add(new MyTicketViewModel
            {
                TicketId = t.TicketId,
                SerialNumber = t.SerialNumber,
                EventTitle = t.TicketType?.Event?.Title ?? string.Empty,
                TicketTypeName = t.TicketType?.TypeName ?? string.Empty,
                Price = actualPrice,
                Status = t.Status,
                IssuedAt = t.IssuedAt
            });
        }
        vm.TotalTickets = vm.Tickets.Count;
        vm.AssignedTickets = vm.Tickets.Count(x => x.Status == "Assigned");
        vm.UsedTickets = vm.Tickets.Count(x => x.Status == "Used");
        vm.RefundedTickets = vm.Tickets.Count(x => x.Status == "Refunded");
        return vm;
    }

    public async Task<TicketQrViewModel?> GetTicketQrAsync(int ticketId, int customerId)
    {
        var t = await _tickets.GetByIdAsync(ticketId);
        if (t == null) return null;
        if (t.OrderItem?.Order?.CustomerId != customerId) return null;
        return new TicketQrViewModel
        {
            TicketId = t.TicketId,
            SerialNumber = t.SerialNumber,
            EventTitle = t.TicketType?.Event?.Title ?? string.Empty,
            EventTime = t.TicketType?.Event?.StartTime ?? DateTime.MinValue,
            Location = t.TicketType?.Event?.Location?.LocationName ?? string.Empty,
            Status = t.Status,
            QrCodeText = t.QrCodeText
        };
    }

    public string GenerateSerialNumber(int eventId, int ticketTypeId)
    {
        var rnd = new Random();
        var tail = rnd.Next(0, 10000).ToString("D4");
        return $"EVT{eventId}-TYPE{ticketTypeId}-{DateTime.Now:yyyyMMddHHmmss}-{tail}";
    }

    public string GenerateQrCodeText(Ticket ticket, int eventId)
        => $"QR|TicketId={ticket.TicketId}|Serial={ticket.SerialNumber}|Event={eventId}";
}
