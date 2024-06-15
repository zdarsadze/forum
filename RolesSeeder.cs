using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public static class RolesSeeder
{
    public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
        }

        if (!await roleManager.RoleExistsAsync("user"))
        {
            await roleManager.CreateAsync(new IdentityRole("user"));
        }
    }
}
