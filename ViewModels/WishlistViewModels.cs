namespace EventManagement.ViewModels;

public class WishlistItemViewModel
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class WishlistPageViewModel
{
    public List<WishlistItemViewModel> Items { get; set; } = new();
}
