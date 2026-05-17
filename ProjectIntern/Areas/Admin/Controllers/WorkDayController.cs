using Microsoft.AspNetCore.Mvc;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.Requests;
using ProjectIntern.Web.ViewModels.Admin.WorkDay;
using System.Security.Claims;

namespace ProjectIntern.Areas.Admin.Controllers;

public class WorkDayController : BaseAdminController
{
    private readonly IWorkDayService workDayService;
    private readonly ILogger<WorkDayController> logger;

    public WorkDayController(IWorkDayService workDayService, ILogger<WorkDayController> logger)
    {
        this.workDayService = workDayService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Calendar(Guid internId)
    {
        try
        {
            InternCalendarAdminViewModel model = await workDayService.GetWorkDaysForInternAsAdminAsync(internId);
            return View(model);
        }
        catch (ArgumentException)
        {
            TempData["ErrorMessage"] = "Intern not found.";
            return RedirectToAction("AllUsers", "User");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading calendar for intern {InternId}", internId);
            TempData["ErrorMessage"] = "Could not load the calendar at this time.";
            return RedirectToAction("AllUsers", "User");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDays([FromBody] WorkDayBatchRequestModel request)
    {
        if (request == null || !request.Dates.Any())
            return BadRequest("No dates provided.");

        Guid adminId = GetCurrentUserId();

        try
        {
            IEnumerable<DateTime> dates = request.Dates
                .Select(d => DateTime.SpecifyKind(DateTime.Parse(d), DateTimeKind.Utc));

            await workDayService.CreateWorkDayAsync(request.InternId, dates, adminId);
            return Ok(new { message = "Work days added successfully." });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Business rule violation adding days for intern {InternId}", request.InternId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding work days for intern {InternId}", request.InternId);
            return StatusCode(500, new { message = "An error occurred while adding work days." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveDays([FromBody] WorkDayBatchRequestModel request)
    {
        if (request == null || !request.Dates.Any())
            return BadRequest("No dates provided.");

        Guid adminId = GetCurrentUserId();

        try
        {
            IEnumerable<DateTime> dates = request.Dates
                .Select(d => DateTime.SpecifyKind(DateTime.Parse(d), DateTimeKind.Utc));

            await workDayService.DeleteWorkDaysAsync(request.InternId, dates, adminId);
            return Ok(new { message = "Work days removed successfully." });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Business rule violation removing days for intern {InternId}", request.InternId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing work days for intern {InternId}", request.InternId);
            return StatusCode(500, new { message = "An error occurred while removing work days." });
        }
    }

    private Guid GetCurrentUserId()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid adminId))
            throw new InvalidOperationException("Could not resolve current admin user ID.");

        return adminId;
    }
}
