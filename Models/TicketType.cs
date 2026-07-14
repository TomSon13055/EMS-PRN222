using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models;

public class TicketType
{
    public int TicketTypeId { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    [Required, StringLength(100)]
    public string TypeName { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime SaleStart { get; set; }
    public DateTime SaleEnd { get; set; }

    [Required, StringLength(20)]
    public string Status { get; set; } = "Active";
}
