using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models;

public class Ticket
{
    public int TicketId { get; set; }

    public int TicketTypeId { get; set; }
    public TicketType? TicketType { get; set; }

    public int OrderItemId { get; set; }
    public OrderItem? OrderItem { get; set; }

    [Required, StringLength(100)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Status { get; set; } = "Assigned";

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UsedAt { get; set; }
    public DateTime? RefundedAt { get; set; }

    [Required, StringLength(500)]
    public string QrCodeText { get; set; } = string.Empty;
}
