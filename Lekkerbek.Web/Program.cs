using Lekkerbek.Web.Data;
using Lekkerbek.Web.NewFolder;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using Lekkerbek.Web.Jobs;

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
builder.Services.AddTransient<ChefRepository>();
builder.Services.AddTransient<ChefService>();




//Congig connection DataBase
builder.Services.AddDbContext<LekkerbekContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LekkerbekContext>();





// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});


/// Quartz job triger

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("SendEmailJob");
    q.AddJob<SendEmailJob>(opts => opts.WithIdentity(jobKey));
    
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailJob-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)
        .WithCronSchedule("0 * * ? * *")
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddScoped<SendEmailJob>();

var app = builder.Build();


//var serviceProvider = app.Services.CreateScope().ServiceProvider;
//var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//await roleManager.CreateAsync(new IdentityRole("Administrator"));
//await roleManager.CreateAsync(new IdentityRole("Cashier"));
//await roleManager.CreateAsync(new IdentityRole("Chef"));
//await roleManager.CreateAsync(new IdentityRole("Customer"));


//var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
//var adminUser = await userManager.FindByEmailAsync("gip_admin@gmail.com");
//await userManager.AddToRoleAsync(adminUser, "Administrator");





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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
