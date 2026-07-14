using EventManagement.Helpers;
using EventManagement.Services;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[RequireRole("Customer")]
public class TicketController : Controller
{
    private readonly IOrderService _orders;
    private readonly ITicketService _tickets;
    public TicketController(IOrderService orders, ITicketService tickets)
    {
        _orders = orders;
        _tickets = tickets;
    }

    [HttpGet]
    public async Task<IActionResult> Buy(int eventId, int? selectedTicketTypeId, int? qty, string? voucherCode)
    {
        // #region debug log H1 H3
    var uid = HttpContext.GetUserId();
    var role = HttpContext.GetRole();
    System.IO.File.AppendAllText("debug-1d493a.log",
        System.Text.Json.JsonSerializer.Serialize(new {
            id = "log_" + System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_h1",
            timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            location = "TicketController.cs:21",
            message = "Buy GET entry",
            data = new { eventId, userId = uid, role, isLoggedIn = uid.HasValue },
            runId = "run1",
            hypothesisId = "H1"
        }) + "\n");
    // #endregion

        if (eventId <= 0) return RedirectToAction("List", "Event");
        var vm = await _orders.BuildBuyViewModelAsync(eventId, selectedTicketTypeId, qty ?? 1);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Buy(BuyTicketViewModel model)
    {
        // #region debug log H1
        var uid = HttpContext.GetUserId();
        var role = HttpContext.GetRole();
        System.IO.File.AppendAllText("debug-1d493a.log",
            System.Text.Json.JsonSerializer.Serialize(new {
                id = "log_" + System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_h1_post",
                timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                location = "TicketController.cs:47",
                message = "Buy POST entry",
                data = new { model.EventId, model.TicketTypeId, model.Quantity, userId = uid, role, modelStateValid = ModelState.IsValid },
                runId = "run1",
                hypothesisId = "H1"
            }) + "\n");
        // #endregion

        var customerId = HttpContext.GetUserId() ?? 0;
        if (model.Quantity <= 0)
        {
            ModelState.AddModelError(nameof(model.Quantity), "Quantity must be greater than 0");
        }
        if (model.TicketTypeId == null)
        {
            ModelState.AddModelError(nameof(model.TicketTypeId), "Please select a ticket type");
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var (ok, err) = await _orders.BuyTicketAsync(model, customerId);
        if (!ok)
        {
            TempData["Error"] = err;
            return RedirectToAction("Buy", new { eventId = model.EventId });
        }
        TempData["Success"] = "Ticket purchased successfully.";
        return RedirectToAction("MyTickets");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyVoucher(BuyTicketViewModel model)
    {
        var customerId = HttpContext.GetUserId() ?? 0;
        var (ok, err, fresh) = await _orders.ApplyVoucherAsync(model, customerId);
        if (!ok)
        {
            if (fresh != null) fresh.VoucherMessage = err ?? "Apply voucher failed";
            return View("Buy", fresh);
        }
        return View("Buy", fresh);
    }

    [HttpGet]
    public async Task<IActionResult> MyTickets(string? status, string? search)
    {
        var customerId = HttpContext.GetUserId() ?? 0;
        var vm = await _tickets.GetMyTicketsAsync(customerId, status, search);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> QR(int id)
    {
        var customerId = HttpContext.GetUserId() ?? 0;
        var vm = await _tickets.GetTicketQrAsync(id, customerId);
        if (vm == null)
        {
            TempData["Error"] = "Ticket not found or not owned by you.";
            return RedirectToAction("MyTickets");
        }
        return View(vm);
    }
}
