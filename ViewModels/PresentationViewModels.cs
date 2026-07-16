namespace EventManagement.ViewModels;

public class BreadcrumbItemViewModel
{
    public string Text { get; set; } = string.Empty;
    public string? Url { get; set; }
}

public class PageHeaderViewModel
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Eyebrow { get; set; }
    public string? ActionLabel { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionIcon { get; set; }
    public List<BreadcrumbItemViewModel>? Breadcrumbs { get; set; }
}

public class AlertViewModel
{
    public string Type { get; set; } = "info";
    public string? Title { get; set; }
    public string? Message { get; set; }
}

public class EmptyStateViewModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? IconVariant { get; set; }
    public string? ActionLabel { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionIcon { get; set; }
    public string? SecondaryActionLabel { get; set; }
    public string? SecondaryActionUrl { get; set; }
}

public class PaginationViewModel
{
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public object? Query { get; set; }
    public string PageQuery { get; set; } = "page";
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalItems { get; set; }
    public int PageSize { get; set; } = 20;
}

public class VoucherStatusViewModel
{
    public DateTime Now { get; set; } = DateTime.Now;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }
}