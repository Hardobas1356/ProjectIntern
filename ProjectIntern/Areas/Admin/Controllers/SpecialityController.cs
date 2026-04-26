using Microsoft.AspNetCore.Mvc;
using ProjectIntern.Services.Core;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.InternshipSpeciality;

namespace ProjectIntern.Areas.Admin.Controllers
{
    public class SpecialityController : BaseAdminController
    {
        private readonly ISpecialityService specialityService;
        private readonly ILogger<SpecialityController> logger;
        public SpecialityController(ISpecialityService specialityService, ILogger<SpecialityController> logger)
        {
            this.specialityService = specialityService;
            this.logger = logger;
        }

        [HttpGet]

        public async Task<IActionResult> Index(string? searchTerm, bool showDeleted = false, int pageNumber = 1)
        {
            try
            {
                int pageSize = 10;
                var result = await specialityService.GetAllSpecialitiesAsync(pageNumber, pageSize, searchTerm, showDeleted);

                ViewData["SearchTerm"] = searchTerm;
                ViewData["ShowDeleted"] = showDeleted;

                return View(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching specialities in Admin Area.");
                TempData["ErrorMessage"] = "Could not load specialities at this time.";
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public async Task<ActionResult> Details(Guid id)
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return View(new InternshipSpecialityCreateInputModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InternshipSpecialityCreateInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning("Speciality creation form is invalid.");

                    return View(inputModel);
                }

                await specialityService
                    .CreateSpecialityAsync(inputModel);

                return RedirectToAction(nameof(Index));
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating speciality in Admin Area.");
                TempData["ErrorMessage"] = "Could not create speciality at this time.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SoftDeleteSpeciality(Guid id)
        {
            try
            {
                await specialityService.SoftDeleteSpecialityAsync(id);
                TempData["SuccessMessage"] = "Speciality has been successfully deactivated.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error deleting speciality {id}");
                TempData["ErrorMessage"] = "Failed to delete the speciality.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RestoreSpeciality(Guid id)
        {
            try
            {
                await specialityService.RestoreSpecialityAsync(id);
                TempData["SuccessMessage"] = "Speciality has been successfully restored.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error restoring speciality {id}");
                TempData["ErrorMessage"] = "Failed to restore the speciality.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
