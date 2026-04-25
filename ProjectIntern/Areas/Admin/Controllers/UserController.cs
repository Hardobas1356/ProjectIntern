using Microsoft.AspNetCore.Mvc;
using ProjectIntern.Services.Core.Interfaces;

namespace ProjectIntern.Areas.Admin.Controllers;

public class UserController : BaseAdminController
{
    private IApplicationUserService applicationUserService;
    private ILogger<UserController> logger;

    public UserController(IApplicationUserService applicationUserService, ILogger<UserController> logger)
    {
        this.applicationUserService = applicationUserService;
        this.logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> AllUsers(string? searchTerm, bool showDeleted, int pageNumber = 1)
    {
        try
        {
            int pageSize = 10;
            var result = await applicationUserService.GetAllUsersAdminAsync(pageNumber, pageSize, searchTerm, showDeleted);

            ViewData["SearchTerm"] = searchTerm;
            ViewData["ShowDeleted"] = showDeleted;
            return View(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching users in Admin Area.");
            TempData["ErrorMessage"] = "Could not load users at this time.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDeleteUser(Guid id)
    {
        try
        {
            await applicationUserService.SoftDeleteUserAsync(id);
            TempData["SuccessMessage"] = "User has been successfully deactivated.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error deleting user {id}");
            TempData["ErrorMessage"] = "Failed to delete the user.";
        }

        return RedirectToAction(nameof(AllUsers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreUser(Guid id)
    {
        try
        {
            await applicationUserService.RestoreUserAsync(id);
            TempData["SuccessMessage"] = "User has been successfully restored.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error restoring user {id}");
            TempData["ErrorMessage"] = "Failed to restore the user.";
        }

        return RedirectToAction(nameof(AllUsers));
    }
}
