namespace EventManagement.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalHosts { get; set; }
    public int TotalEvents { get; set; }
    public int TotalPublishedEvents { get; set; }
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class AdminAccountListItemViewModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminAccountsPageViewModel
{
    public List<AdminAccountListItemViewModel> Accounts { get; set; } = new();
}
