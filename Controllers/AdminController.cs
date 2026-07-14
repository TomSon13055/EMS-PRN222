using EventManagement.Helpers;
using EventManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[RequireRole("Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _service;
    public AdminController(IAdminService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var vm = await _service.GetDashboardAsync();
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Accounts()
    {
        var vm = await _service.GetAccountsAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAccount(int userId)
    {
        var adminId = HttpContext.GetUserId() ?? 0;
        var (ok, err) = await _service.ToggleAccountAsync(userId, adminId);
        if (!ok) TempData["Error"] = err;
        else TempData["Success"] = "Account status updated.";
        return RedirectToAction("Accounts");
    }
}
