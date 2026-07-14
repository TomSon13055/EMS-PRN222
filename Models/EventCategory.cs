using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models;

public class EventCategory
{
    public int EventCategoryId { get; set; }

    [Required, StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;
}
