namespace EventManagement.Models;

public class Wishlist
{
    public int WishlistId { get; set; }

    public int CustomerId { get; set; }
    public User? Customer { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
