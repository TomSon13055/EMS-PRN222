using EventManagement.Helpers;
using EventManagement.Services;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[RequireRole("Host")]
public class TicketTypeController : Controller
{
    private readonly ITicketTypeService _service;
    private readonly IEventService _events;
    public TicketTypeController(ITicketTypeService service, IEventService events)
    {
        _service = service;
        _events = events;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int eventId)
    {
        var hostId = HttpContext.GetUserId() ?? 0;
        var ev = await _events.GetEventDetailAsync(eventId);
        if (ev == null)
        {
            TempData["Error"] = "Event not found.";
            return RedirectToAction("MyEvents", "Host");
        }
        if (ev.HostId != hostId)
        {
            TempData["Error"] = "You do not have permission to manage ticket types for this event.";
            return RedirectToAction("MyEvents", "Host");
        }

        var vm = new CreateTicketTypeViewModel
        {
            EventId = eventId,
            EventTitle = ev.Title,
            SaleStart = DateTime.Now,
            SaleEnd = ev.StartTime.AddHours(-1),
            Status = "Active"
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTicketTypeViewModel model)
    {
        var hostId = HttpContext.GetUserId() ?? 0;
        if (!ModelState.IsValid) return View(model);
        var (ok, err) = await _service.CreateTicketTypeAsync(model, hostId);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err ?? "Create failed");
            return View(model);
        }
        TempData["Success"] = "Ticket type created successfully.";
        return RedirectToAction("MyEvents", "Host");
    }
}
