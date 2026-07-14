namespace EventManagement.ViewModels;

public class WalletPageViewModel
{
    public decimal Balance { get; set; }
    public List<WalletTransactionViewModel> Transactions { get; set; } = new();
}

public class WalletTransactionViewModel
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = "";
    public string Status { get; set; } = "";
    public string? Description { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedAt { get; set; }
}
