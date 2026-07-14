using EventManagement.Models;

namespace EventManagement.Repositories;

public interface IVoucherRepository
{
    Task<Voucher?> GetByCodeAsync(string code);
    Task<bool> CodeExistsAsync(string code);
    Task AddAsync(Voucher voucher);
    Task<List<Voucher>> GetByHostAsync(int hostId);
    Task UpdateAsync(Voucher voucher);
    Task<List<Voucher>> GetAllAsync();
}
