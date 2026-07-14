using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models;

public class EventLocation
{
    public int EventLocationId { get; set; }

    [Required, StringLength(150)]
    public string LocationName { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Address { get; set; }
}
