using InternSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectIntern.Data.Seeding;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        RoleManager<IdentityRole<Guid>> roleManager
            = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        UserManager<ApplicationUser> userManager
            = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
        }

        string adminEmail = "admin@test.com";
        string adminUserName = "Admin";
        ApplicationUser? existingAdmin = await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin == null)
        {
            ApplicationUser adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                Name = "System Admin",
                EmailConfirmed = true,
                CreationDate = DateTime.UtcNow,
                // Note: Specialty and University are null for the Admin
                InternshipSpecialityId = null
            };

            IdentityResult result = await userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
