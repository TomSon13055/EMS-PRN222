using EventManagement.Helpers;
using EventManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[RequireRole("Customer")]
public class WishlistController : Controller
{
    private readonly IWishlistService _service;
    public WishlistController(IWishlistService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var customerId = HttpContext.GetUserId() ?? 0;
        var vm = await _service.GetWishlistAsync(customerId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int eventId)
    {
        var customerId = HttpContext.GetUserId() ?? 0;
        var (ok, msg) = await _service.AddAsync(eventId, customerId);
        if (!ok) TempData["Error"] = msg;
        else TempData["Success"] = "Added to wishlist.";
        return RedirectToAction("List", "Event", new { selectedEventId = eventId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int eventId)
    {
        var customerId = HttpContext.GetUserId() ?? 0;
        await _service.RemoveAsync(eventId, customerId);
        TempData["Success"] = "Removed from wishlist.";
        return RedirectToAction("List");
    }
}
