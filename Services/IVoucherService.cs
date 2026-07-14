using EventManagement.ViewModels;

namespace EventManagement.Services;

public interface IVoucherService
{
    Task<(bool Ok, string? Error)> CreateVoucherAsync(CreateVoucherViewModel model, int hostId);
    Task<VoucherValidationResult> ValidateAsync(string code, decimal originalAmount);
    Task<List<VoucherListItemViewModel>> GetByHostAsync(int hostId);
}
