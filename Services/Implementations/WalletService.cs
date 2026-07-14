using EventManagement.Data;
using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services.Implementations;

public class WalletService : IWalletService
{
    private readonly ApplicationDbContext _db;
    private readonly IUserRepository _users;

    public WalletService(ApplicationDbContext db, IUserRepository users)
    {
        _db = db;
        _users = users;
    }

    public async Task<(bool Ok, string? Error)> TopUpAsync(int userId, decimal amount)
    {
        if (amount <= 0)
            return (false, "Amount must be greater than 0");

        var user = await _users.GetByIdAsync(userId);
        if (user == null)
            return (false, "User not found");

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var before = user.WalletBalance;
            user.WalletBalance += amount;
            user.UpdatedAt = DateTime.Now;
            _db.Users.Update(user);

            _db.WalletTransactions.Add(new WalletTransaction
            {
                UserId = userId,
                Amount = amount,
                TransactionType = "TopUp",
                Status = "Completed",
                Description = $"Top up wallet",
                BalanceBefore = before,
                BalanceAfter = user.WalletBalance,
                CreatedAt = DateTime.Now
            });

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
            return (true, null);
        }
        catch
        {
            await tx.RollbackAsync();
            return (false, "Unable to process your request. Please try again later");
        }
    }

    public async Task<WalletPageViewModel> GetWalletPageAsync(int userId)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
        var txns = await _db.WalletTransactions.AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(50)
            .ToListAsync();

        return new WalletPageViewModel
        {
            Balance = user?.WalletBalance ?? 0m,
            Transactions = txns.Select(t => new WalletTransactionViewModel
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                Status = t.Status,
                Description = t.Description,
                BalanceBefore = t.BalanceBefore,
                BalanceAfter = t.BalanceAfter,
                CreatedAt = t.CreatedAt
            }).ToList()
        };
    }
}
