using EventManagement.Helpers;
using EventManagement.Services;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[RequireRole("Host")]
public class HostController : Controller
{
    private readonly IEventService _events;
    public HostController(IEventService events) { _events = events; }

    [HttpGet]
    public async Task<IActionResult> MyEvents(string? status, string? search)
    {
        var hostId = HttpContext.GetUserId() ?? 0;
        var vm = await _events.GetHostMyEventsAsync(hostId, status, search);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> CreateEvent()
    {
        var vm = new CreateEventViewModel
        {
            StartTime = DateTime.Now.AddDays(7).Date.AddHours(19),
            EndTime = DateTime.Now.AddDays(7).Date.AddHours(21),
            Status = "Draft"
        };
        await PopulateSelectsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectsAsync(model);
            return View(model);
        }
        var hostId = HttpContext.GetUserId() ?? 0;
        var (ok, err, evId) = await _events.CreateEventAsync(model, hostId);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err ?? "Create failed");
            await PopulateSelectsAsync(model);
            return View(model);
        }
        TempData["Success"] = "Event created successfully.";
        return RedirectToAction("Create", "TicketType", new { eventId = evId });
    }

    private async Task PopulateSelectsAsync(CreateEventViewModel vm)
    {
        var page = await _events.GetEventListPageAsync(new EventFilterViewModel(), null, "Guest");
        vm.Categories = page.Categories;
        vm.Locations = page.Locations;
        vm.Statuses = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
        {
            new() { Value = "Draft", Text = "Draft" },
            new() { Value = "Published", Text = "Published" }
        };
    }
}
