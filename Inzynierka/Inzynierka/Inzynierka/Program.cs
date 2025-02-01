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

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/*builder.Services.AddDefaultIdentity<Users>(options => options.SignIn.RequireConfirmedAccount = true)// IdentityUser
    .AddEntityFrameworkStores<AhoyDbContext>();
builder.Services.AddControllersWithViews();*/

builder.Services.AddIdentity<Users, Roles>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // Wymaganie potwierdzenia konta

    // Konfiguracja wymagań dotyczących hasła
    options.Password.RequireDigit = true; // Wymaga co najmniej jednej cyfry w haśle
    options.Password.RequiredLength = 8; // Minimalna długość hasła to 8 znaków
    options.Password.RequireNonAlphanumeric = true; // Wymaga co najmniej jednego znaku specjalnego w haśle
    options.Password.RequireUppercase = true; // Wymaga co najmniej jednej wielkiej litery w haśle
    options.Password.RequireLowercase = true; // Wymaga co najmniej jednej małej litery w haśle

    // Konfiguracja zasad blokowania konta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Konto zostanie zablokowane na 5 minut po przekroczeniu limitu nieudanych prób logowania
    options.Lockout.MaxFailedAccessAttempts = 5; // Maksymalna liczba nieudanych prób logowania to 5
    options.Lockout.AllowedForNewUsers = true; // Nowi użytkownicy również mogą mieć blokowane konto

    // Konfiguracja wymagań dotyczących użytkownika
    options.User.RequireUniqueEmail = true; // Wymaga unikalnego adresu e-mail dla każdego użytkownika
    /*options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; */ // Dozwolone znaki w nazwach użytkowników

    // Konfiguracja wymagań dotyczących rejestracji i logowania
    //options.SignIn.RequireConfirmedAccount = true; // Wymaga, aby konto użytkownika było potwierdzone przed zalogowaniem
    //options.SignIn.RequireConfirmedEmail = true; // Wymaga potwierdzenia adresu e-mail przed zalogowaniem
})

.AddEntityFrameworkStores<AhoyDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10); // Czas trwania sesji (10 minut)
    options.SlidingExpiration = true; // Odnawia czas wygaśnięcia przy każdej aktywności użytkownika
    options.LoginPath = "/Account/Login"; // Ścieżka do strony logowania
    options.LogoutPath = "/Account/Logout"; // Ścieżka do wylogowania
    options.AccessDeniedPath = "/Account/AccessDenied"; // Ścieżka do strony dostępu zabronionego
});

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


using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await RoleInitializer.CreateRoles(serviceProvider);
}

/*// Użyj sesji
app.UseSession();

// Middleware do sprawdzania wygasłej sesji
app.Use(async (context, next) =>
{
    // Jeśli sesja jest pusta (np. użytkownik nie jest zalogowany), przekieruj na stronę główną
    if (context.Session.GetString("UserSession") == null)
    {
        context.Response.Redirect("/Home/Index");
    }
    else
    {
        await next.Invoke();
    }
});*/

app.Run();
