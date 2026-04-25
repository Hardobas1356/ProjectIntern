using InternSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectIntern.Data.Seeding
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Ensure the Admin Role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            // 2. Define the Admin user
            var adminEmail = "admin@test.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "System Admin",
                    EmailConfirmed = true,
                    CreationDate = DateTime.UtcNow,
                    // Note: Specialty and University are null for the Admin
                    InternshipSpecialityId = null
                };

                // 3. Create the user with a default password
                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    // 4. Assign the Admin Role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
