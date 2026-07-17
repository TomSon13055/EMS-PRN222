using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManagement.ViewModels;

public class BuyTicketViewModel
{
    public int EventId { get; set; }
    public string? EventTitle { get; set; }
    public string? EventDescription { get; set; }
    public DateTime? EventStartTime { get; set; }
    public string? LocationName { get; set; }

    [Required(ErrorMessage = "Please select a ticket type")]
    [Display(Name = "Ticket Type")]
    public int? TicketTypeId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; } = 1;

    [Display(Name = "Voucher Code")]
    public string? VoucherCode { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? VoucherMessage { get; set; }
    public bool VoucherApplied { get; set; }
    public int VoucherDiscountPercent { get; set; }
    public decimal? WalletBalance { get; set; }
    public List<SelectListItem> TicketTypes { get; set; } = new();
}

public class MyTicketViewModel
{
    public int TicketId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string EventTitle { get; set; } = string.Empty;
    public string TicketTypeName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
}

public class MyTicketsPageViewModel
{
    public List<MyTicketViewModel> Tickets { get; set; } = new();
    public int TotalTickets { get; set; }
    public int AssignedTickets { get; set; }
    public int UsedTickets { get; set; }
    public int RefundedTickets { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }
    public List<SelectListItem> Statuses { get; set; } = new();
}

public class TicketQrViewModel
{
    public int TicketId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string EventTitle { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string QrCodeText { get; set; } = string.Empty;
}
