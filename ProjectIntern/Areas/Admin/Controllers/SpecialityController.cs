using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<ActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                InternshipSpecialityDetailsViewModel viewModel
                    = await specialityService.GetSpecialityDetailsAsync(id);

                if (viewModel == null)
                {
                    TempData["ErrorMessage"] = "The requested speciality could not be found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching details for Speciality {SpecialityId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the details.";
                return RedirectToAction(nameof(Index));
            }
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

        [HttpGet]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                InternshipSpecialityEditInputModel model = await specialityService.GetSpecialityForEditAsync(id);
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while getting speciality for edit in Admin Area.");
                TempData["ErrorMessage"] = "Could not edit speciality at this time.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(InternshipSpecialityEditInputModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning("Speciality edit form is invalid.");
                    return View(model);
                }

                await specialityService.EditSpecialityAsync(model);
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while editing speciality in Admin Area.");
                TempData["ErrorMessage"] = "Could not save the edited speciality at this time.";
                return RedirectToAction(nameof(Index));
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromBody] ReorderRequest model)
        {
            try
            {
                // specialityId can be passed via URL or inside the JSON body
                await specialityService.ReorderTopicsAsync(model.SpecialityId, model.TopicIds);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest("Could not reorder topics.");
            }
        }

        // Simple DTO for the request
        public class ReorderRequest
        {
            public Guid SpecialityId { get; set; }
            public List<Guid> TopicIds { get; set; }
        }
    }
}
