using EventManagement.Data;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Repositories.Implementations;

public class VoucherRepository : IVoucherRepository
{
    private readonly ApplicationDbContext _db;
    public VoucherRepository(ApplicationDbContext db) { _db = db; }

    public Task<Voucher?> GetByCodeAsync(string code) =>
        _db.Vouchers.FirstOrDefaultAsync(v => v.VoucherCode == code);

    public Task<bool> CodeExistsAsync(string code) =>
        _db.Vouchers.AnyAsync(v => v.VoucherCode == code);

    public async Task AddAsync(Voucher voucher)
    {
        _db.Vouchers.Add(voucher);
        await _db.SaveChangesAsync();
    }

    public Task<List<Voucher>> GetByHostAsync(int hostId) =>
        _db.Vouchers.AsNoTracking()
            .Where(v => v.CreatedByUserId == hostId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

    public async Task UpdateAsync(Voucher voucher)
    {
        _db.Vouchers.Update(voucher);
        await _db.SaveChangesAsync();
    }

    public Task<List<Voucher>> GetAllAsync() =>
        _db.Vouchers.AsNoTracking().OrderBy(v => v.VoucherId).ToListAsync();
}
