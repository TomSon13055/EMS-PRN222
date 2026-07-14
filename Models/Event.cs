using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models;

public class Event
{
    public int EventId { get; set; }

    public int HostId { get; set; }
    public User? Host { get; set; }

    public int EventCategoryId { get; set; }
    public EventCategory? Category { get; set; }

    public int EventLocationId { get; set; }
    public EventLocation? Location { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [Required, StringLength(20)]
    public string Status { get; set; } = "Draft";

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
