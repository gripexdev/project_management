using Microsoft.AspNetCore.Identity;

namespace ProjectDashboard.Data
{
    public static class RoleSeeder
    {
        public static async Task seedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}