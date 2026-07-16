using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManagement.ViewModels;

public class EventFilterViewModel
{
    public int? CategoryId { get; set; }
    public int? LocationId { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }
    public int? SelectedEventId { get; set; }
}

public class TicketTypeViewModel
{
    public int TicketTypeId { get; set; }
    public int EventId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int Sold { get; set; }
    public int Available { get; set; }
    public DateTime SaleStart { get; set; }
    public DateTime SaleEnd { get; set; }
    public string Status { get; set; } = "Active";
}

public class EventDetailViewModel
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public int HostId { get; set; }
    public string? ImageUrl { get; set; }
    public List<TicketTypeViewModel> TicketTypes { get; set; } = new();
    public int TicketTypeCount => TicketTypes.Count;
    public int TotalAvailableTickets => TicketTypes.Sum(t => t.Available);
}

public class EventListItemViewModel
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public int HostId { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? MinPrice { get; set; }
    public int? AvailableTickets { get; set; }
    public bool CanEdit { get; set; }
    public bool CanBuy { get; set; }
    public bool CanWishlist { get; set; }
}

public class EventCardViewModel
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal? MinPrice { get; set; }
    public int? AvailableTickets { get; set; }
    public bool CanEdit { get; set; }
    public bool CanBuy { get; set; }
    public bool CanWishlist { get; set; }

    public static EventCardViewModel FromListItem(EventListItemViewModel item) => new()
    {
        EventId = item.EventId,
        Title = item.Title,
        CategoryName = item.CategoryName,
        LocationName = item.LocationName,
        Status = item.Status,
        HostName = item.HostName,
        ImageUrl = item.ImageUrl,
        StartTime = item.StartTime,
        EndTime = item.EndTime,
        MinPrice = item.MinPrice,
        AvailableTickets = item.AvailableTickets,
        CanEdit = item.CanEdit,
        CanBuy = item.CanBuy,
        CanWishlist = item.CanWishlist
    };
}

public class EventListPageViewModel
{
    public List<EventListItemViewModel> Events { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();
    public List<SelectListItem> Statuses { get; set; } = new();
    public EventDetailViewModel? SelectedEvent { get; set; }
    public EventFilterViewModel Filter { get; set; } = new();
}

public class CreateEventViewModel
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int EventCategoryId { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [Display(Name = "Location")]
    public int EventLocationId { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    [Display(Name = "Start Time")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    [Display(Name = "End Time")]
    public DateTime EndTime { get; set; }

    [StringLength(500)]
    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    [Required]
    public string Status { get; set; } = "Draft";

    public List<SelectListItem>? Categories { get; set; }
    public List<SelectListItem>? Locations { get; set; }
    public List<SelectListItem>? Statuses { get; set; }
}

public class EditEventViewModel
{
    public int EventId { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int EventCategoryId { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [Display(Name = "Location")]
    public int EventLocationId { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    [Display(Name = "Start Time")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    [Display(Name = "End Time")]
    public DateTime EndTime { get; set; }

    [StringLength(500)]
    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    [Required]
    public string Status { get; set; } = "Draft";

    public List<SelectListItem>? Categories { get; set; }
    public List<SelectListItem>? Locations { get; set; }
    public List<SelectListItem>? Statuses { get; set; }
    public List<TicketTypeViewModel> TicketTypes { get; set; } = new();
}

public class HostMyEventItemViewModel
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TicketSold { get; set; }
    public decimal Revenue { get; set; }
}

public class HostMyEventsPageViewModel
{
    public List<HostMyEventItemViewModel> Events { get; set; } = new();
    public string? Status { get; set; }
    public string? Search { get; set; }
    public List<SelectListItem> Statuses { get; set; } = new();
}
