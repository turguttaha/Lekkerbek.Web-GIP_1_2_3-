using Lekkerbek.Web.Data;
using Lekkerbek.Web.NewFolder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages()
// Maintain property names during serialization. See:
// https://github.com/aspnet/Announcements/issues/194
.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver =
new Newtonsoft.Json.Serialization.DefaultContractResolver());
// Add Kendo UI services to the services container
builder.Services.AddKendo();


//Congig connection DataBase
builder.Services.AddDbContext<LekkerbekContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<CustomerService>();



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
