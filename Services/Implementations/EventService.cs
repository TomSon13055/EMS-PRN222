using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services.Implementations;

public class EventService : IEventService
{
    private readonly IEventRepository _events;
    private readonly ITicketTypeRepository _ticketTypes;

    public EventService(IEventRepository events, ITicketTypeRepository ticketTypes)
    {
        _events = events;
        _ticketTypes = ticketTypes;
    }

    public async Task<EventListPageViewModel> GetEventListPageAsync(EventFilterViewModel filter, int? currentUserId, string? role)
    {
        var q = _events.GetEventsQuery();

        if (string.IsNullOrEmpty(role) || role == "Guest" || role == "Customer")
        {
            q = q.Where(e => e.Status == "Published");
        }
        else if (role == "Host" && currentUserId.HasValue)
        {
            q = q.Where(e => e.Status == "Published" || e.HostId == currentUserId.Value);
        }

        if (filter.CategoryId.HasValue)
            q = q.Where(e => e.EventCategoryId == filter.CategoryId.Value);
        if (filter.LocationId.HasValue)
            q = q.Where(e => e.EventLocationId == filter.LocationId.Value);
        if (!string.IsNullOrEmpty(filter.Status))
            q = q.Where(e => e.Status == filter.Status);
        if (!string.IsNullOrEmpty(filter.Search))
        {
            var s = filter.Search.Trim();
            q = q.Where(e => e.Title.Contains(s) || e.Description.Contains(s));
        }

        var list = await q.OrderBy(e => e.StartTime).ToListAsync();

        var vm = new EventListPageViewModel
        {
            Filter = filter,
            Categories = (await _events.GetCategoriesAsync())
                .Select((c, i) => new SelectListItem { Value = c.EventCategoryId.ToString(), Text = c.CategoryName }).ToList(),
            Locations = (await _events.GetLocationsAsync())
                .Select((l, i) => new SelectListItem { Value = l.EventLocationId.ToString(), Text = l.LocationName }).ToList(),
            Statuses = new List<SelectListItem>
            {
                new() { Value = "Published", Text = "Published" },
                new() { Value = "Draft", Text = "Draft" },
                new() { Value = "Ongoing", Text = "Ongoing" },
                new() { Value = "Completed", Text = "Completed" },
                new() { Value = "Cancelled", Text = "Cancelled" }
            }
        };

        vm.Categories.Insert(0, new SelectListItem { Value = "", Text = "All Categories" });
        vm.Locations.Insert(0, new SelectListItem { Value = "", Text = "All Locations" });
        vm.Statuses.Insert(0, new SelectListItem { Value = "", Text = "All Status" });

        var isCustomer = role == "Customer";
        var isHost = role == "Host";
        var isAdmin = role == "Admin";

        foreach (var e in list)
        {
            var types = await _ticketTypes.GetByEventIdAsync(e.EventId);
            decimal? minPrice = null;
            int? available = null;
            foreach (var t in types)
            {
                if (!minPrice.HasValue || t.Price < minPrice.Value) minPrice = t.Price;
                var sold = await _ticketTypes.CountSoldAsync(t.TicketTypeId);
                var left = Math.Max(0, t.Quantity - sold);
                available = (available ?? 0) + left;
            }

            vm.Events.Add(new EventListItemViewModel
            {
                EventId = e.EventId,
                Title = e.Title,
                CategoryName = e.Category?.CategoryName ?? string.Empty,
                LocationName = e.Location?.LocationName ?? string.Empty,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Status = e.Status,
                HostName = e.Host?.FullName ?? string.Empty,
                HostId = e.HostId,
                ImageUrl = e.ImageUrl,
                MinPrice = minPrice,
                AvailableTickets = available,
                CanBuy = isCustomer && e.Status == "Published",
                CanWishlist = isCustomer && e.Status == "Published",
                CanEdit = isAdmin || (isHost && currentUserId == e.HostId)
            });
        }

        if (filter.SelectedEventId.HasValue)
            vm.SelectedEvent = await GetEventDetailAsync(filter.SelectedEventId.Value);

        return vm;
    }

    public async Task<EventDetailViewModel?> GetEventDetailAsync(int eventId)
    {
        var ev = await _events.GetByIdAsync(eventId);
        if (ev == null) return null;
        var types = await _ticketTypes.GetByEventIdAsync(eventId);
        var vm = new EventDetailViewModel
        {
            EventId = ev.EventId,
            Title = ev.Title,
            Description = ev.Description,
            CategoryName = ev.Category?.CategoryName ?? string.Empty,
            LocationName = ev.Location?.LocationName ?? string.Empty,
            Address = ev.Location?.Address,
            StartTime = ev.StartTime,
            EndTime = ev.EndTime,
            Status = ev.Status,
            HostName = ev.Host?.FullName ?? string.Empty,
            HostId = ev.HostId,
            ImageUrl = ev.ImageUrl
        };
        foreach (var t in types)
        {
            var sold = await _ticketTypes.CountSoldAsync(t.TicketTypeId);
            vm.TicketTypes.Add(new TicketTypeViewModel
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
        return vm;
    }

    public async Task<(bool Ok, string? Error, int? Id)> CreateEventAsync(CreateEventViewModel model, int hostId)
    {
        if (model.StartTime <= DateTime.Now)
            return (false, "Start time cannot be in the past", null);
        if (model.EndTime <= model.StartTime)
            return (false, "End time must be after start time", null);

        var ev = new Event
        {
            HostId = hostId,
            EventCategoryId = model.EventCategoryId,
            EventLocationId = model.EventLocationId,
            Title = model.Title,
            Description = model.Description,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Status = model.Status,
            ImageUrl = model.ImageUrl,
            CreatedAt = DateTime.Now
        };
        await _events.AddAsync(ev);
        return (true, null, ev.EventId);
    }

    public async Task<(bool Ok, string? Error, EditEventViewModel? Model)> GetEditViewModelAsync(int eventId, int userId, string role)
    {
        var ev = await _events.GetByIdAsync(eventId);
        if (ev == null) return (false, "Event not found", null);
        var isAdmin = role == "Admin";
        var isOwner = role == "Host" && ev.HostId == userId;
        if (!isAdmin && !isOwner) return (false, "You do not have permission to edit this event", null);

        var categories = await _events.GetCategoriesAsync();
        var locations = await _events.GetLocationsAsync();
        var ticketTypes = await _ticketTypes.GetByEventIdAsync(eventId);
        var model = new EditEventViewModel
        {
            EventId = ev.EventId,
            Title = ev.Title,
            Description = ev.Description,
            EventCategoryId = ev.EventCategoryId,
            EventLocationId = ev.EventLocationId,
            StartTime = ev.StartTime,
            EndTime = ev.EndTime,
            Status = ev.Status,
            ImageUrl = ev.ImageUrl,
            Categories = categories.Select(c => new SelectListItem { Value = c.EventCategoryId.ToString(), Text = c.CategoryName }).ToList(),
            Locations = locations.Select(l => new SelectListItem { Value = l.EventLocationId.ToString(), Text = l.LocationName }).ToList(),
            Statuses = new List<SelectListItem>
            {
                new() { Value = "Draft", Text = "Draft" },
                new() { Value = "Published", Text = "Published" },
                new() { Value = "Cancelled", Text = "Cancelled" }
            },
            TicketTypes = ticketTypes.Select(t => new TicketTypeViewModel
            {
                TicketTypeId = t.TicketTypeId,
                EventId = t.EventId,
                TypeName = t.TypeName,
                Price = t.Price,
                Quantity = t.Quantity,
                Sold = 0,
                Available = t.Quantity,
                SaleStart = t.SaleStart,
                SaleEnd = t.SaleEnd,
                Status = t.Status
            }).ToList()
        };
        return (true, null, model);
    }

    public async Task<(bool Ok, string? Error)> UpdateEventAsync(EditEventViewModel model, int userId, string role)
    {
        var ev = await _events.GetByIdAsync(model.EventId);
        if (ev == null) return (false, "Event not found");
        var isAdmin = role == "Admin";
        var isOwner = role == "Host" && ev.HostId == userId;
        if (!isAdmin && !isOwner) return (false, "You do not have permission to edit this event");

        if (model.EndTime <= model.StartTime)
            return (false, "End time must be after start time");

        var hasSold = await _events.HasSoldTicketsAsync(ev.EventId);
        if (hasSold)
        {
            if (model.StartTime != ev.StartTime || model.EndTime != ev.EndTime)
                return (false, "Cannot change start/end time after tickets are sold");
            if (model.Status == "Cancelled")
                return (false, "Cannot cancel event after tickets are sold");
        }
        else
        {
            if (model.StartTime <= DateTime.Now)
                return (false, "Start time cannot be in the past");
        }

        ev.Title = model.Title;
        ev.Description = model.Description;
        ev.EventCategoryId = model.EventCategoryId;
        ev.EventLocationId = model.EventLocationId;
        ev.StartTime = model.StartTime;
        ev.EndTime = model.EndTime;
        ev.Status = model.Status;
        ev.ImageUrl = model.ImageUrl;
        ev.UpdatedAt = DateTime.Now;
        await _events.UpdateAsync(ev);
        return (true, null);
    }

    public async Task<HostMyEventsPageViewModel> GetHostMyEventsAsync(int hostId, string? status, string? search)
    {
        var q = _events.GetEventsQuery().Where(e => e.HostId == hostId);
        if (!string.IsNullOrEmpty(status))
            q = q.Where(e => e.Status == status);
        if (!string.IsNullOrEmpty(search))
        {
            var s = search.Trim();
            q = q.Where(e => e.Title.Contains(s));
        }
        var list = await q.OrderByDescending(e => e.CreatedAt).ToListAsync();

        var vm = new HostMyEventsPageViewModel
        {
            Status = status,
            Search = search,
            Statuses = new List<SelectListItem>
            {
                new() { Value = "", Text = "All Status" },
                new() { Value = "Draft", Text = "Draft" },
                new() { Value = "Published", Text = "Published" },
                new() { Value = "Ongoing", Text = "Ongoing" },
                new() { Value = "Completed", Text = "Completed" },
                new() { Value = "Cancelled", Text = "Cancelled" }
            }
        };
        foreach (var e in list)
        {
            var sold = await _events.CountSoldTicketsAsync(e.EventId);
            var revenue = await _events.SumRevenueAsync(e.EventId);
            vm.Events.Add(new HostMyEventItemViewModel
            {
                EventId = e.EventId,
                Title = e.Title,
                Category = e.Category?.CategoryName ?? string.Empty,
                Location = e.Location?.LocationName ?? string.Empty,
                StartTime = e.StartTime,
                Status = e.Status,
                TicketSold = sold,
                Revenue = revenue
            });
        }
        return vm;
    }
}
