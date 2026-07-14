using EventManagement.Helpers;
using EventManagement.Services;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

public class EventController : Controller
{
    private readonly IEventService _events;
    public EventController(IEventService events) { _events = events; }

    [HttpGet]
    public async Task<IActionResult> List(EventFilterViewModel filter)
    {
        var role = HttpContext.GetRole();
        var userId = HttpContext.GetUserId();
        var page = await _events.GetEventListPageAsync(filter, userId, role);
        return View(page);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = HttpContext.GetUserId() ?? 0;
        var role = HttpContext.GetRole() ?? string.Empty;
        if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Account");
        var (ok, err, model) = await _events.GetEditViewModelAsync(id, userId, role);
        if (!ok)
        {
            TempData["Error"] = err;
            return RedirectToAction("List");
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditEventViewModel model)
    {
        var userId = HttpContext.GetUserId() ?? 0;
        var role = HttpContext.GetRole() ?? string.Empty;
        if (!ModelState.IsValid)
        {
            var (okReload, _, freshModel) = await _events.GetEditViewModelAsync(model.EventId, userId, role);
            if (okReload && freshModel != null)
            {
                model.Categories = freshModel.Categories;
                model.Locations = freshModel.Locations;
                model.Statuses = freshModel.Statuses;
            }
            return View(model);
        }
        var (ok2, err) = await _events.UpdateEventAsync(model, userId, role);
        if (!ok2)
        {
            ModelState.AddModelError(string.Empty, err ?? "Update failed");
            var (okReload, _, freshModel) = await _events.GetEditViewModelAsync(model.EventId, userId, role);
            if (okReload && freshModel != null)
            {
                model.Categories = freshModel.Categories;
                model.Locations = freshModel.Locations;
                model.Statuses = freshModel.Statuses;
            }
            return View(model);
        }
        TempData["Success"] = "Event updated successfully.";
        return RedirectToAction("List");
    }
}
