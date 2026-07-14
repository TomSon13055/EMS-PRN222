using EventManagement.Data;
using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManagement.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _db;
    private readonly IUserRepository _users;
    private readonly IEventRepository _events;
    private readonly ITicketTypeRepository _ticketTypes;
    private readonly IVoucherRepository _vouchers;
    private readonly INotificationService _notifications;
    private readonly ITicketService _tickets;

    public OrderService(ApplicationDbContext db, IUserRepository users, IEventRepository events,
        ITicketTypeRepository ticketTypes, IVoucherRepository vouchers, INotificationService notifications,
        ITicketService tickets)
    {
        _db = db;
        _users = users;
        _events = events;
        _ticketTypes = ticketTypes;
        _vouchers = vouchers;
        _notifications = notifications;
        _tickets = tickets;
    }

    public async Task<BuyTicketViewModel> BuildBuyViewModelAsync(int eventId)
    {
        var ev = await _events.GetByIdAsync(eventId);
        var types = await _ticketTypes.GetByEventIdAsync(eventId);
        Console.WriteLine($"[DEBUG] BuildBuyViewModelAsync: eventId={eventId}, ev={ev?.Title}, types_count={types.Count}");
        foreach (var t in types)
            Console.WriteLine($"[DEBUG]   TicketType: {t.TicketTypeId} | {t.TypeName} | Price={t.Price} | Qty={t.Quantity} | Status={t.Status}");
        // #region debug log H2
        System.IO.File.AppendAllText("debug-1d493a.log",
            System.Text.Json.JsonSerializer.Serialize(new {
                id = "log_" + System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_h2",
                timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                location = "OrderService.cs:34",
                message = "BuildBuyViewModelAsync",
                data = new { eventId, eventTitle = ev?.Title, typesCount = types.Count,
                    types = types.Select(t => new { t.TicketTypeId, t.TypeName, t.Status, t.SaleStart, t.SaleEnd, t.Quantity }) },
                runId = "run1",
                hypothesisId = "H2"
            }) + "\n");
        // #endregion
        var vm = new BuyTicketViewModel
        {
            EventId = eventId,
            EventTitle = ev?.Title,
            EventDescription = ev?.Description,
            EventStartTime = ev?.StartTime,
            LocationName = ev?.Location?.LocationName,
            TicketTypes = types.Select(t => new SelectListItem
            {
                Value = t.TicketTypeId.ToString(),
                Text = $"{t.TypeName} - {t.Price:N0} đ"
            }).ToList()
        };
        return vm;
    }

    public async Task<BuyTicketViewModel> BuildBuyViewModelAsync(int eventId, int? ticketTypeId, int quantity)
    {
        var vm = await BuildBuyViewModelAsync(eventId);
        vm.TicketTypeId = ticketTypeId;
        vm.Quantity = quantity;
        if (ticketTypeId.HasValue && quantity > 0)
        {
            var tt = await _ticketTypes.GetByIdAsync(ticketTypeId.Value);
            if (tt != null && tt.EventId == eventId)
            {
                vm.SubTotal = tt.Price * quantity;
                vm.FinalAmount = vm.SubTotal;
            }
        }
        return vm;
    }

    public async Task<(bool Ok, string? Error, BuyTicketViewModel? Model)> ApplyVoucherAsync(BuyTicketViewModel model, int customerId)
    {
        var fresh = await BuildBuyViewModelAsync(model.EventId);
        fresh.TicketTypeId = model.TicketTypeId;
        fresh.Quantity = model.Quantity;
        fresh.VoucherCode = model.VoucherCode;

        if (!fresh.EventStartTime.HasValue)
        {
            fresh.VoucherMessage = "Unable to validate event date";
            return (false, "Unable to validate event date", fresh);
        }

        if (model.TicketTypeId == null)
        {
            fresh.VoucherMessage = "Please select a ticket type first";
            return (false, "Please select a ticket type first", fresh);
        }
        if (model.Quantity <= 0)
        {
            fresh.VoucherMessage = "Quantity must be greater than 0";
            return (false, "Quantity must be greater than 0", fresh);
        }

        var tt = await _ticketTypes.GetByIdAsync(model.TicketTypeId.Value);
        if (tt == null || tt.EventId != model.EventId)
        {
            fresh.VoucherMessage = "Invalid ticket type";
            return (false, "Invalid ticket type", fresh);
        }

        if (DateTime.Now > fresh.EventStartTime.Value)
        {
            fresh.VoucherMessage = "Voucher is no longer valid for this event";
            return (false, "Voucher is no longer valid for this event", fresh);
        }

        var sub = tt.Price * model.Quantity;
        fresh.SubTotal = sub;
        fresh.DiscountAmount = 0;
        fresh.FinalAmount = sub;

        if (!string.IsNullOrWhiteSpace(model.VoucherCode))
        {
            var v = await _vouchers.GetByCodeAsync(model.VoucherCode.Trim());
        var effectiveVoucherTo = v.ValidTo;
        if (fresh.EventStartTime.HasValue && fresh.EventStartTime.Value < v.ValidTo)
        {
            effectiveVoucherTo = fresh.EventStartTime.Value;
        }

        if (v == null || !v.IsActive || DateTime.Now < v.ValidFrom || DateTime.Now > effectiveVoucherTo || v.UsedCount >= v.UsageLimit)
        {
            fresh.VoucherMessage = "Invalid or expired voucher code";
            return (false, "Invalid or expired voucher code", fresh);
        }
            var discount = Math.Round(sub * v.DiscountPercentage / 100m, 2);
            fresh.DiscountAmount = discount;
            fresh.FinalAmount = sub - discount;
            fresh.VoucherApplied = true;
            fresh.VoucherDiscountPercent = (int)v.DiscountPercentage;
            fresh.VoucherMessage = $"Voucher applied: {v.VoucherCode} (-{v.DiscountPercentage}%)";
        }

        return (true, null, fresh);
    }

    public async Task<(bool Ok, string? Error)> BuyTicketAsync(BuyTicketViewModel model, int customerId)
    {
        // #region debug log H4
        var ttLookup = await _ticketTypes.GetByIdAsync(model.TicketTypeId ?? 0);
        var _now = DateTime.Now;
        var checks = new {
            ticketTypeFound = ttLookup != null,
            eventIdMatch = ttLookup?.EventId == model.EventId,
            statusActive = ttLookup?.Status == "Active",
            saleWindowOk = ttLookup != null && _now >= ttLookup.SaleStart && _now <= ttLookup.SaleEnd,
            qty = model.Quantity,
            now = _now,
            saleStart = ttLookup?.SaleStart,
            saleEnd = ttLookup?.SaleEnd,
            customerId
        };
        System.IO.File.AppendAllText("debug-1d493a.log",
            System.Text.Json.JsonSerializer.Serialize(new {
                id = "log_" + System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_h4",
                timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                location = "OrderService.cs:103",
                message = "BuyTicketAsync validation",
                data = checks,
                runId = "run1",
                hypothesisId = "H4"
            }) + "\n");
        // #endregion
        if (model.TicketTypeId == null) return (false, "Please select a ticket type");
        if (model.Quantity <= 0) return (false, "Quantity must be greater than 0");

        var tt = await _ticketTypes.GetByIdAsync(model.TicketTypeId.Value);
        if (tt == null || tt.EventId != model.EventId) return (false, "Invalid ticket type");
        if (tt.Status != "Active") return (false, "Ticket type is not available");
        var now = DateTime.Now;
        if (now < tt.SaleStart || now > tt.SaleEnd) return (false, "Ticket type is not in sale window");

        var sold = await _ticketTypes.CountSoldAsync(tt.TicketTypeId);
        var available = tt.Quantity - sold;
        if (model.Quantity > available) return (false, "Not enough tickets available");

        var sub = tt.Price * model.Quantity;
        var discount = 0m;
        Voucher? voucher = null;
        if (!string.IsNullOrWhiteSpace(model.VoucherCode))
        {
            voucher = await _vouchers.GetByCodeAsync(model.VoucherCode.Trim());
            if (voucher == null || !voucher.IsActive || now < voucher.ValidFrom || now > voucher.ValidTo || voucher.UsedCount >= voucher.UsageLimit)
                return (false, "Invalid or expired voucher code");
            discount = Math.Round(sub * voucher.DiscountPercentage / 100m, 2);
        }
        var final = sub - discount;

        var customer = await _users.GetByIdAsync(customerId);
        if (customer == null) return (false, "Customer not found");
        if (customer.WalletBalance < final) return (false, "Insufficient wallet balance");

        var ev = await _events.GetByIdAsync(model.EventId);
        if (ev == null) return (false, "Event not found");

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                CustomerId = customerId,
                TotalAmount = sub,
                DiscountAmount = discount,
                FinalAmount = final,
                Status = "Paid",
                VoucherId = voucher?.VoucherId,
                CreatedAt = now,
                PaidAt = now
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            var item = new OrderItem
            {
                OrderId = order.OrderId,
                TicketTypeId = tt.TicketTypeId,
                Quantity = model.Quantity,
                UnitPrice = tt.Price,
                SubTotal = sub
            };
            _db.OrderItems.Add(item);
            await _db.SaveChangesAsync();

            var before = customer.WalletBalance;
            customer.WalletBalance = before - final;
            customer.UpdatedAt = now;
            _db.Users.Update(customer);

            _db.WalletTransactions.Add(new WalletTransaction
            {
                UserId = customerId,
                Amount = final,
                TransactionType = "Payment",
                Status = "Completed",
                Description = $"Payment for order #{order.OrderId} - {ev.Title}",
                BalanceBefore = before,
                BalanceAfter = customer.WalletBalance,
                CreatedAt = now
            });

            if (voucher != null)
            {
                voucher.UsedCount += 1;
                voucher.UpdatedAt = now;
                _db.Vouchers.Update(voucher);
            }

            var newTickets = new List<Ticket>();
            for (int i = 0; i < model.Quantity; i++)
            {
                var serial = _tickets.GenerateSerialNumber(ev.EventId, tt.TicketTypeId);
                var newTicket = new Ticket
                {
                    OrderItemId = item.OrderItemId,
                    TicketTypeId = tt.TicketTypeId,
                    SerialNumber = serial,
                    Status = "Assigned",
                    IssuedAt = now,
                    QrCodeText = "" // set after save
                };
                newTickets.Add(newTicket);
            }
            _db.Tickets.AddRange(newTickets);
            await _db.SaveChangesAsync();

            foreach (var t in newTickets)
                t.QrCodeText = _tickets.GenerateQrCodeText(t, ev.EventId);
            _db.Tickets.UpdateRange(newTickets);
            await _db.SaveChangesAsync();

            await _notifications.CreateAsync(customerId,
                "Ticket Purchase Successful",
                $"You have successfully purchased ticket(s) for {ev.Title}.");

            await tx.CommitAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine($"[ERROR] BuyTicketAsync exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            return (false, "Unable to process your request. Please try again later");
        }
    }
}
