namespace EventManagement.ViewModels;

public class NotificationItemViewModel
{
    public int UserNotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationPageViewModel
{
    public List<NotificationItemViewModel> Items { get; set; } = new();
}
