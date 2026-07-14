using EventManagement.Helpers;
using EventManagement.Services;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[RequireRole("Customer")]
public class WalletController : Controller
{
    private readonly IWalletService _wallet;
    public WalletController(IWalletService wallet) => _wallet = wallet;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.GetUserId() ?? 0;
        var vm = await _wallet.GetWalletPageAsync(userId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TopUp(decimal amount)
    {
        var userId = HttpContext.GetUserId() ?? 0;
        var (ok, err) = await _wallet.TopUpAsync(userId, amount);
        if (!ok)
        {
            TempData["Error"] = err;
        }
        else
        {
            TempData["Success"] = $"Top up successful. Your new balance is ready.";
        }
        return RedirectToAction("Index");
    }
}
