using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<EventLocation> EventLocations => Set<EventLocation>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<TicketType> TicketTypes => Set<TicketType>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Voucher> Vouchers => Set<Voucher>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.WalletBalance).HasColumnType("decimal(18,2)");
        });

        b.Entity<Event>(e =>
        {
            e.HasOne(x => x.Host).WithMany().HasForeignKey(x => x.HostId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.EventCategoryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.EventLocationId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<TicketType>(e =>
        {
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
        });

        b.Entity<Order>(e =>
        {
            e.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.FinalAmount).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Voucher).WithMany().HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<OrderItem>(e =>
        {
            e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
        });

        b.Entity<Ticket>(e =>
        {
            e.HasIndex(x => x.SerialNumber).IsUnique();
            e.HasOne(x => x.OrderItem).WithMany().HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.TicketType).WithMany().HasForeignKey(x => x.TicketTypeId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Voucher>(e =>
        {
            e.HasIndex(x => x.VoucherCode).IsUnique();
            e.Property(x => x.DiscountPercentage).HasColumnType("decimal(5,2)");
        });

        b.Entity<Wishlist>(e =>
        {
            e.HasIndex(x => new { x.CustomerId, x.EventId }).IsUnique();
        });

        b.Entity<WalletTransaction>(e =>
        {
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.BalanceBefore).HasColumnType("decimal(18,2)");
            e.Property(x => x.BalanceAfter).HasColumnType("decimal(18,2)");
        });
    }
}
