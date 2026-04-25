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

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> AllUsers(string? searchTerm, int pageNumber = 1)
    {
        try
        {
            int pageSize = 10;
            var result = await applicationUserService.GetAllUsersAdminAsync(pageNumber, pageSize, searchTerm);

            ViewData["SearchTerm"] = searchTerm;
            return View(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching users in Admin Area.");
            TempData["ErrorMessage"] = "Could not load users at this time.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
