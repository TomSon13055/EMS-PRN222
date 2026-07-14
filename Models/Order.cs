using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models;

public class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }
    public User? Customer { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; } = 0m;
    public decimal FinalAmount { get; set; }

    [Required, StringLength(20)]
    public string Status { get; set; } = "Pending";

    public int? VoucherId { get; set; }
    public Voucher? Voucher { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
