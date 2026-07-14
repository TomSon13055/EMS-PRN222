using EventManagement.Data;
using EventManagement.Repositories;
using EventManagement.Repositories.Implementations;
using EventManagement.Services;
using EventManagement.Services.Implementations;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromHours(2);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITicketTypeService, TicketTypeService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IWalletService, WalletService>();

var app = builder.Build();

// DB schema and seed data are managed via setup.sql (run manually in SSMS)

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Event/List");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Event}/{action=List}/{id?}");

app.MapGet("/debug/tickettypes/{eventId}", async (int eventId, ApplicationDbContext db,
    ITicketTypeRepository repo) =>
{
    var direct = await db.TicketTypes.AsNoTracking().Where(t => t.EventId == eventId).ToListAsync();
    var viaRepo = await repo.GetByEventIdAsync(eventId);
    return Results.Json(new
    {
        eventId,
        direct_count = direct.Count,
        viaRepo_count = viaRepo.Count,
        direct = direct.Select(t => new { t.TicketTypeId, t.EventId, t.TypeName, t.Price, t.Status }),
        viaRepo = viaRepo.Select(t => new { t.TicketTypeId, t.EventId, t.TypeName, t.Price, t.Status })
    });
});

app.MapGet("/debug/buy/{eventId}", async (int eventId, IOrderService orders) =>
{
    var vm = await orders.BuildBuyViewModelAsync(eventId);
    return Results.Json(new
    {
        eventId,
        eventTitle = vm.EventTitle,
        ticketTypes_count = vm.TicketTypes?.Count ?? -1,
        ticketTypes = vm.TicketTypes
    });
});

app.MapGet("/debug/trace/{eventId}", async (int eventId, ApplicationDbContext db,
    ITicketTypeRepository repo, IEventRepository events) =>
{
    var ev = await events.GetByIdAsync(eventId);
    var types = await repo.GetByEventIdAsync(eventId);
    var selectList = types.Select(t => new SelectListItem
    {
        Value = t.TicketTypeId.ToString(),
        Text = $"{t.TypeName} - {t.Price:N0} đ"
    }).ToList();

    var vm = new BuyTicketViewModel
    {
        EventId = eventId,
        EventTitle = ev?.Title,
        EventDescription = ev?.Description,
        EventStartTime = ev?.StartTime,
        LocationName = ev?.Location?.LocationName,
        TicketTypes = selectList
    };

    return Results.Json(new
    {
        eventId,
        eventTitle = vm.EventTitle,
        ticketTypes_count = vm.TicketTypes?.Count ?? -1,
        vm_TicketTypes = vm.TicketTypes?.Select(t => new { t.Value, t.Text }) ?? Enumerable.Empty<object>()
    });
});

app.Run();
