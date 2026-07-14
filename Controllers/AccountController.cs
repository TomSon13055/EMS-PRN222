using EventManagement.Helpers;
using EventManagement.Services;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _auth;
    public AccountController(IAuthService auth) { _auth = auth; }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var (ok, err) = await _auth.RegisterAsync(model);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err ?? "Registration failed");
            return View(model);
        }
        TempData["Success"] = "Registration successful. Please log in.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var (user, err) = await _auth.LoginAsync(model);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, err ?? "Login failed");
            return View(model);
        }
        HttpContext.SetUser(user.UserId, user.FullName, user.Role);
        return user.Role switch
        {
            "Admin" => RedirectToAction("Dashboard", "Admin"),
            "Host" => RedirectToAction("MyEvents", "Host"),
            _ => RedirectToAction("List", "Event")
        };
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.ClearUser();
        return RedirectToAction("List", "Event");
    }
}
