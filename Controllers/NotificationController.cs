using EventManagement.Helpers;
using EventManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[RequireRole("Customer", "Host", "Admin")]
public class NotificationController : Controller
{
    private readonly INotificationService _service;
    public NotificationController(INotificationService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var userId = HttpContext.GetUserId() ?? 0;
        var vm = await _service.GetByUserAsync(userId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        var userId = HttpContext.GetUserId() ?? 0;
        await _service.MarkAsReadAsync(id, userId);
        return RedirectToAction("List");
    }
}
