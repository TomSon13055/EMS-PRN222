using EventManagement.Repositories;
using EventManagement.ViewModels;

namespace EventManagement.Services.Implementations;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _vouchers;
    public VoucherService(IVoucherRepository vouchers) { _vouchers = vouchers; }

    public async Task<(bool Ok, string? Error)> CreateVoucherAsync(CreateVoucherViewModel model, int hostId)
    {
        if (model.DiscountPercentage <= 0 || model.DiscountPercentage > 100)
            return (false, "Discount percentage must be between 0 and 100");
        if (model.ValidTo <= model.ValidFrom)
            return (false, "Valid to must be after valid from");
        if (model.ValidFrom < DateTime.Today)
            return (false, "Valid from cannot be in the past");
        if (model.ValidTo > DateTime.Today.AddYears(1))
            return (false, "Valid to cannot be more than 1 year in the future");
        if (model.UsageLimit <= 0)
            return (false, "Usage limit must be greater than 0");
        if (await _vouchers.CodeExistsAsync(model.VoucherCode))
            return (false, "Voucher code already exists");

        await _vouchers.AddAsync(new Models.Voucher
        {
            CreatedByUserId = hostId,
            VoucherCode = model.VoucherCode,
            DiscountPercentage = model.DiscountPercentage,
            ValidFrom = model.ValidFrom,
            ValidTo = model.ValidTo,
            UsageLimit = model.UsageLimit,
            IsActive = model.IsActive,
            UsedCount = 0,
            CreatedAt = DateTime.Now
        });
        return (true, null);
    }

    public async Task<VoucherValidationResult> ValidateAsync(string code, decimal originalAmount)
    {
        if (string.IsNullOrWhiteSpace(code))
            return new VoucherValidationResult { IsValid = false, Message = "Voucher code is required" };
        var v = await _vouchers.GetByCodeAsync(code.Trim());
        if (v == null)
            return new VoucherValidationResult { IsValid = false, Message = "Invalid or expired voucher code" };
        var now = DateTime.Now;
        if (!v.IsActive || now < v.ValidFrom || now > v.ValidTo || v.UsedCount >= v.UsageLimit)
            return new VoucherValidationResult { IsValid = false, Message = "Invalid or expired voucher code" };
        var discount = Math.Round(originalAmount * v.DiscountPercentage / 100m, 2);
        return new VoucherValidationResult
        {
            IsValid = true,
            DiscountPercentage = v.DiscountPercentage,
            DiscountAmount = discount,
            FinalAmount = originalAmount - discount,
            VoucherId = v.VoucherId,
            Message = "Voucher applied"
        };
    }

    public async Task<List<VoucherListItemViewModel>> GetByHostAsync(int hostId)
    {
        var list = await _vouchers.GetByHostAsync(hostId);
        return list.Select(v => new VoucherListItemViewModel
        {
            VoucherId = v.VoucherId,
            VoucherCode = v.VoucherCode,
            DiscountPercentage = v.DiscountPercentage,
            ValidFrom = v.ValidFrom,
            ValidTo = v.ValidTo,
            UsageLimit = v.UsageLimit,
            UsedCount = v.UsedCount,
            IsActive = v.IsActive
        }).ToList();
    }
}
