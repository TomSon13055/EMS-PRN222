using System.ComponentModel.DataAnnotations;

namespace EventManagement.ViewModels;

public class CreateTicketTypeViewModel
{
    public int EventId { get; set; }
    public string? EventTitle { get; set; }

    [Required(ErrorMessage = "Type name is required")]
    [StringLength(100)]
    [Display(Name = "Type Name")]
    public string TypeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Sale start is required")]
    [Display(Name = "Sale Start")]
    public DateTime SaleStart { get; set; }

    [Required(ErrorMessage = "Sale end is required")]
    [Display(Name = "Sale End")]
    public DateTime SaleEnd { get; set; }

    [Required]
    public string Status { get; set; } = "Active";
}
