using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.ViewModels;

namespace EventManagement.Services.Implementations;

public class TicketTypeService : ITicketTypeService
{
    private readonly ITicketTypeRepository _ticketTypes;
    private readonly IEventRepository _events;

    public TicketTypeService(ITicketTypeRepository ticketTypes, IEventRepository events)
    {
        _ticketTypes = ticketTypes;
        _events = events;
    }

    public async Task<(bool Ok, string? Error)> CreateTicketTypeAsync(CreateTicketTypeViewModel model, int hostId)
    {
        var ev = await _events.GetByIdAsync(model.EventId);
        if (ev == null) return (false, "Event not found");
        if (ev.HostId != hostId) return (false, "You do not have permission to add ticket type to this event");

        if (model.Price <= 0) return (false, "Price must be greater than 0");
        if (model.Quantity <= 0) return (false, "Quantity must be greater than 0");
        if (model.SaleEnd <= model.SaleStart) return (false, "Sale end must be after sale start");
        if (model.SaleEnd > ev.StartTime) return (false, "Sale end must be before or equal to event start time");

        var entity = new TicketType
        {
            EventId = model.EventId,
            TypeName = model.TypeName,
            Price = model.Price,
            Quantity = model.Quantity,
            SaleStart = model.SaleStart,
            SaleEnd = model.SaleEnd,
            Status = model.Status
        };
        await _ticketTypes.AddAsync(entity);
        return (true, null);
    }

    public async Task<List<TicketTypeViewModel>> GetByEventAsync(int eventId)
    {
        var list = await _ticketTypes.GetByEventIdAsync(eventId);
        var result = new List<TicketTypeViewModel>();
        foreach (var t in list)
        {
            var sold = await _ticketTypes.CountSoldAsync(t.TicketTypeId);
            result.Add(new TicketTypeViewModel
            {
                TicketTypeId = t.TicketTypeId,
                EventId = t.EventId,
                TypeName = t.TypeName,
                Price = t.Price,
                Quantity = t.Quantity,
                Sold = sold,
                Available = Math.Max(0, t.Quantity - sold),
                SaleStart = t.SaleStart,
                SaleEnd = t.SaleEnd,
                Status = t.Status
            });
        }
        return result;
    }
}
