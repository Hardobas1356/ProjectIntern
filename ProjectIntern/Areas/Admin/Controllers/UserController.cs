using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.ApplicationUser;
using System.Security.Claims;

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
    public async Task<IActionResult> Index(string? searchTerm, bool showDeleted, int pageNumber = 1)
    {
        try
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
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

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            UserEditInputModel model = await applicationUserService.GetUserForEditAsync(id);
            return View(model);
        }
        catch (KeyNotFoundException)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error fetching user {id} for edit.");
            TempData["ErrorMessage"] = "An error occurred while loading the user data.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(UserEditInputModel model)
    {
        if (!ModelState.IsValid)
        {
            await RefreshSpecialitiesAsync(model);
            return View("Edit", model);
        }

        try
        {
            await applicationUserService.EditUserAsync(model);
            TempData["SuccessMessage"] = "User details updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error updating user {model.Id}.");
            TempData["ErrorMessage"] = "Failed to update user details.";

            await RefreshSpecialitiesAsync(model);
            return View("Edit", model);
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

        return RedirectToAction(nameof(Index));
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

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDeleteUser(Guid id)
    {
        try
        {
            await applicationUserService.HardDeleteUserAsync(id);
            TempData["SuccessMessage"] = "User has been permanently deleted.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error permanently deleting user {id}");
            TempData["ErrorMessage"] = "Failed to permanently delete the user.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeAdmin(Guid id)
    {
        try
        {
            await applicationUserService.MakeAdminAsync(id);
            TempData["SuccessMessage"] = "User has been granted admin privileges.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error granting admin privileges to user {id}");
            TempData["ErrorMessage"] = "Failed to grant admin privileges.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAdmin(Guid id)
    {
        Guid actingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            await applicationUserService.RemoveAdminAsync(id, actingUserId);
            TempData["SuccessMessage"] = "Admin privileges have been revoked from the user.";
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, $"Cannot remove admin from user {id}");
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error revoking admin privileges from user {id}");
            TempData["ErrorMessage"] = "Failed to revoke admin privileges.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task RefreshSpecialitiesAsync(UserEditInputModel model)
    {
        try
        {
            UserEditInputModel refreshedModel = await applicationUserService.GetUserForEditAsync(model.Id);
            model.Specialities = refreshedModel.Specialities;
        }
        catch
        {
            model.Specialities = new List<SelectListItem>();
        }
    }
}
