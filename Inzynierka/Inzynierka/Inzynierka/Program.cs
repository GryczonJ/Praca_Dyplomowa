using Inzynierka.Data;
using Inzynierka.Data.Tables;
using Inzynierka.Models;
using Inzynierka.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AhoyDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<Users>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AhoyDbContext>();

//builder.Services.AddDefaultIdentity<Users>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AhoyDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/*builder.Services.AddDefaultIdentity<Users>(options => options.SignIn.RequireConfirmedAccount = true)// IdentityUser
    .AddEntityFrameworkStores<AhoyDbContext>();
builder.Services.AddControllersWithViews();*/

builder.Services.AddIdentity<Users, Roles>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Wymaganie potwierdzenia konta
})
.AddEntityFrameworkStores<AhoyDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddAuthorization(); // Rejestracja us�ug autoryzacji
builder.Services.AddControllersWithViews(); // Rejestracja us�ug MVC
builder.Services.AddRazorPages(); // Rejestracja us�ug Razor Pages
/*
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages();

app.Run();
