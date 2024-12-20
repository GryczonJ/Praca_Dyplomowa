using Inzynierka.Data.Tables;
using Microsoft.AspNetCore.Identity;

namespace Inzynierka.Data
{
    public static class RoleInitializer
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Roles>>();
            var roles = new[] { "Kapitan", "User", "Moderacja" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var identityRole = new Roles { Name = role };
                    await roleManager.CreateAsync(identityRole);
                }
            }
        }
    }
}
