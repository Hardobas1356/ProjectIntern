using InternSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace ProjectIntern.Data.Seeding;

public static class DbSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // ==========================================
        // 1. SEED SYSTEM ROLES
        // ==========================================
        string[] roleNames = { "Admin", "Intern" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
            }
        }

        // ==========================================
        // 2. SEED SPECIALITIES & TOPICS FROM JSON
        // ==========================================
        if (!context.InternshipSpecialities.Any())
        {
            string jsonPath = Path.Combine(AppContext.BaseDirectory, "topics-seed.json");

            if (!File.Exists(jsonPath))
            {
                jsonPath = "..\\ProjectIntern.Data.Seeding\\topics-seed.json";
            }

            if (File.Exists(jsonPath))
            {
                try
                {
                    string jsonText = await File.ReadAllTextAsync(jsonPath);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var specialitiesList = JsonSerializer.Deserialize<List<SpecialitySeedModel>>(jsonText, options);

                    if (specialitiesList != null && specialitiesList.Any())
                    {
                        foreach (var specData in specialitiesList)
                        {
                            var speciality = new InternshipSpeciality
                            {
                                Id = Guid.NewGuid(),
                                Name = specData.Speciality,
                                Description = string.IsNullOrWhiteSpace(specData.Description)
                                   ? $"{specData.Speciality} Track"
                                   : specData.Description,
                                IsDeleted = false
                            };
                            context.InternshipSpecialities.Add(speciality);

                            foreach (var topicData in specData.Topics)
                            {
                                context.Topics.Add(new Topic
                                {
                                    Id = Guid.NewGuid(),
                                    InternshipSpecialityId = speciality.Id,
                                    Name = topicData.Name,
                                    Description = topicData.Description,
                                    Order = topicData.Order,
                                    IsDeleted = false
                                });
                            }
                        }
                        await context.SaveChangesAsync();
                        Console.WriteLine("[Seeder Success] Specialities and Topics loaded beautifully!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Seeder Error] Failed parsing JSON topics content: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[Seeder Warning] JSON seed file missing. Searched location: {jsonPath}");
            }
        }

        // ==========================================
        // 3. SEED ADMIN USER
        // ==========================================
        string adminUsername = "admin";
        var adminUser = await userManager.FindByNameAsync(adminUsername);
        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = adminUsername,
                Name = "System Administrator",
                Email = "admin@projectintern.local",
                EmailConfirmed = true,
                CreationDate = DateTime.UtcNow
            };

            var createAdminResult = await userManager.CreateAsync(admin, "Admin123!");
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                Console.WriteLine("[Seeder Success] Admin user established.");
            }
        }
    }
}

public class SpecialitySeedModel
{
    public string Speciality { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<TopicSeedModel> Topics { get; set; } = new();
}
public class TopicSeedModel
{
    public int Order { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}