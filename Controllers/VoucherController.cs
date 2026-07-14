using EventManagement.Helpers;
using EventManagement.Services;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[RequireRole("Host")]
public class VoucherController : Controller
{
    private readonly IVoucherService _service;
    public VoucherController(IVoucherService service) { _service = service; }

    [HttpGet]
    public IActionResult Create()
    {
        var vm = new CreateVoucherViewModel
        {
            ValidFrom = DateTime.Now,
            ValidTo = DateTime.Now.AddMonths(1),
            IsActive = true
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVoucherViewModel model)
    {
        var hostId = HttpContext.GetUserId() ?? 0;
        if (!ModelState.IsValid) return View(model);
        var (ok, err) = await _service.CreateVoucherAsync(model, hostId);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err ?? "Create failed");
            return View(model);
        }
        TempData["Success"] = "Voucher created successfully.";
        return RedirectToAction("List");
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var hostId = HttpContext.GetUserId() ?? 0;
        var list = await _service.GetByHostAsync(hostId);
        return View(list);
    }
}
