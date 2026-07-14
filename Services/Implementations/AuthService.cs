using System.Security.Cryptography;
using System.Text;
using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.ViewModels;

namespace EventManagement.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    public AuthService(IUserRepository users) { _users = users; }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }

    public async Task<(bool Ok, string? Error)> RegisterAsync(RegisterViewModel model)
    {
        if (model.Role != "Customer" && model.Role != "Host")
            return (false, "Role must be Customer or Host.");

        if (await _users.EmailExistsAsync(model.Email))
            return (false, "Email already exists");

        var user = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            PasswordHash = HashPassword(model.Password),
            Role = model.Role,
            WalletBalance = model.Role == "Customer" ? 500_000m : 0m,
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        await _users.AddAsync(user);
        return (true, null);
    }

    public async Task<(User? User, string? Error)> LoginAsync(LoginViewModel model)
    {
        var user = await _users.GetByEmailAsync(model.Email);
        if (user == null || user.PasswordHash != HashPassword(model.Password))
            return (null, "Invalid email or password");
        if (!user.IsActive)
            return (null, "Your account has been locked");
        return (user, null);
    }
}
