using Microsoft.AspNetCore.Mvc;
using ProjectIntern.Services.Core.Interfaces;

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
                var result = await specialityService.GetAllSpecialities(pageNumber, pageSize, searchTerm, showDeleted);

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

        public ActionResult Details(Guid id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        public ActionResult Edit(Guid id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, IFormCollection collection)
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
        public ActionResult Delete(Guid id)
        {
            return View();
        }
    }
}
