using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Web.ViewModels.ApplicationUser;
using System.Security.Claims;

namespace ProjectIntern.Controllers;

[Authorize]
public class CalendarController : Controller
{
    private readonly IWorkDayService workDayService;
    private readonly ILogger<CalendarController> logger;

    public CalendarController(IWorkDayService workDayService, ILogger<CalendarController> logger)
    {
        this.workDayService = workDayService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        Guid internId = GetCurrentUserId();

        try
        {
            InternCalendarViewModel model = await workDayService.GetWorkDaysForInternAsync(internId);
            return View(model);
        }
        catch (ArgumentException)
        {
            TempData["ErrorMessage"] = "Your account could not be found.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading calendar for intern {InternId}", internId);
            TempData["ErrorMessage"] = "Could not load your calendar at this time.";
            return RedirectToAction("Index", "Home");
        }
    }

    private Guid GetCurrentUserId()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid id))
            throw new InvalidOperationException("Could not resolve current user ID.");

        return id;
    }
}