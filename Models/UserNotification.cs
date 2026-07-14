using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models;

public class UserNotification
{
    public int UserNotificationId { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
