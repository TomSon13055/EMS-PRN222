using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManagement.ViewModels;

public class CreateVoucherViewModel
{
    public int? EventId { get; set; }

    [Required(ErrorMessage = "Voucher code is required")]
    [StringLength(50)]
    [Display(Name = "Voucher Code")]
    public string VoucherCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Discount percentage is required")]
    [Range(0.01, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
    [Display(Name = "Discount Percentage")]
    public decimal DiscountPercentage { get; set; }

    [Required(ErrorMessage = "Valid from is required")]
    [Display(Name = "Valid From")]
    public DateTime ValidFrom { get; set; }

    [Required(ErrorMessage = "Valid to is required")]
    [Display(Name = "Valid To")]
    public DateTime ValidTo { get; set; }

    [Required(ErrorMessage = "Usage limit is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Usage limit must be greater than 0")]
    [Display(Name = "Usage Limit")]
    public int UsageLimit { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;
}

public class VoucherValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public int? VoucherId { get; set; }
}

public class VoucherListItemViewModel
{
    public int VoucherId { get; set; }
    public string VoucherCode { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public bool IsActive { get; set; }
}
