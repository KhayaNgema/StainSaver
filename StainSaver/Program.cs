using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StainSaver.Data;
using StainSaver.Models;
using StainSaver.Services;
using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddTransient<ReceiveItemsService>();
builder.Services.AddTransient<FileUploadService>();
builder.Services.AddTransient<SmsService>();
builder.Services.AddTransient<PickUpSmsService>();
builder.Services.AddTransient<BarcodeService>();
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Add Hangfire services and configure to use SQL Server storage
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Add the Hangfire Server to process jobs in background
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Add Hangfire Dashboard middleware (optional, secured as needed)
app.UseHangfireDashboard("/hangfire");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed database and roles/users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations");
        await context.Database.MigrateAsync();

        logger.LogInformation("Seeding roles and users");
        await DbSeeder.SeedRolesAndUsers(services);

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var customer = await userManager.FindByEmailAsync("customer1@gmail.com");
        if (customer != null)
        {
            var isInRole = await userManager.IsInRoleAsync(customer, "Customer");
            logger.LogInformation($"Customer user exists: {customer.Id}, In Customer role: {isInRole}");
        }
        else
        {
            logger.LogWarning("Customer user not found - seeding may have failed");
        }

        logger.LogInformation("Seeding laundry services");
        await DbSeeder.SeedLaundryServices(services);

        logger.LogInformation("Ensuring booking tables exist");
        await DbSeeder.EnsureBookingTablesExist(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.Run();
