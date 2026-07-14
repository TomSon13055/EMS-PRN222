using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models;

public class WalletTransaction
{
    [Key]
    public int TransactionId { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public decimal Amount { get; set; }

    [Required, StringLength(20)]
    public string TransactionType { get; set; } = "Payment";

    [Required, StringLength(20)]
    public string Status { get; set; } = "Completed";

    [StringLength(500)]
    public string? Description { get; set; }

    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
