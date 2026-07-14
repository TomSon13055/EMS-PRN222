using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface IAuthService
{
    Task<(bool Ok, string? Error)> RegisterAsync(RegisterViewModel model);
    Task<(Models.User? User, string? Error)> LoginAsync(LoginViewModel model);
}
