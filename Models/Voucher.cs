using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models;

public class Voucher
{
    public int VoucherId { get; set; }

    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    [Required, StringLength(50)]
    public string VoucherCode { get; set; } = string.Empty;

    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    public int UsageLimit { get; set; }
    public int UsedCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
