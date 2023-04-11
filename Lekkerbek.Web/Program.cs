using Lekkerbek.Web.Data;
using Lekkerbek.Web.NewFolder;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages()
// Maintain property names during serialization. See:
// https://github.com/aspnet/Announcements/issues/194
.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver =
new Newtonsoft.Json.Serialization.DefaultContractResolver());
// Add Kendo UI services to the services container
builder.Services.AddKendo();

//Services And Repositories
builder.Services.AddTransient<CustomersRepository>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<MenuItemRepository>();
builder.Services.AddTransient<IMenuItemService, MenuItemService>();
builder.Services.AddTransient<OrdersRepository>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<OrdersCashierRepository>();
builder.Services.AddTransient<IOrderCashierService, OrderCashierService>();
builder.Services.AddTransient<OrdersChefRepository>();
builder.Services.AddTransient<IOrderChefService, OrderChefService>();

//Congig connection DataBase
builder.Services.AddDbContext<LekkerbekContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));





// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
