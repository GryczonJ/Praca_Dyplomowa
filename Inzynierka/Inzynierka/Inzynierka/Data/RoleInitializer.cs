using Inzynierka.Data.Tables;
using Microsoft.AspNetCore.Identity;

namespace Inzynierka.Data
{
    public class RoleInitializer
    {
        private readonly UserManager<Users> _userManager; // Podaj odpowiedni typ dla użytkownika

        public RoleInitializer(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }
        public async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Roles>>();
            var roles = new[] { "Kapitan", "User", "Moderacja", "Banned" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var identityRole = new Roles { Name = role };
                    await roleManager.CreateAsync(identityRole);
                }
            }
            // Predefined users with their roles
            var predefinedUsers = new[]
            {
            new { UserName = "kapitan@example.com", Email = "kapitan@example.com", Role = "Kapitan", FirstName = "Kapitan", LastName = "Smith", Password = "Password123!" },
            new { UserName = "user@example.com", Email = "user@example.com", Role = "User", FirstName = "Jan", LastName = "Kowalski", Password = "Password123!" },
            new { UserName = "moderator@example.com", Email = "moderator@example.com", Role = "Moderacja", FirstName = "Anna", LastName = "Nowak", Password = "Password123!" },
            new { UserName = "banned@example.com", Email = "banned@example.com", Role = "Banned", FirstName = "John", LastName = "Doe", Password = "Password123!" }
             };

            // Tworzenie użytkowników, jeśli nie istnieją
            foreach (var userInfo in predefinedUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(userInfo.Email); // Używamy _userManager, a nie UserManager
                if (existingUser == null)
                {
                    var user = new Users
                    {
                        UserName = userInfo.UserName,
                        Email = userInfo.Email,
                        EmailConfirmed = true,
                        banned = userInfo.Role == "Banned"
                    };
                    var result = await _userManager.CreateAsync(user, userInfo.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, userInfo.Role);
                    }
                }
            }
        }
    }
}
