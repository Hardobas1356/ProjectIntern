using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using ProjectIntern.Services.Core;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.Topic;

namespace ProjectIntern.Areas.Admin.Controllers;

public class TopicController : BaseAdminController
{
    private readonly ISpecialityService specialityService;
    private readonly ITopicService topicService;
    private readonly ILogger<TopicController> logger;

    public TopicController(ITopicService topicService, ISpecialityService specialityService, ILogger<TopicController> logger)
    {
        this.topicService = topicService;
        this.specialityService = specialityService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid specialityId)
    {
        TopicCreateInputModel model = new TopicCreateInputModel { InternshipSpecialityId = specialityId };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TopicCreateInputModel model)
    {
        try
        {
            if (!ModelState.IsValid) return View(model);

            await topicService.CreateTopicAsync(model, model.InternshipSpecialityId);

            return RedirectToAction("Details", "Speciality", new { id = model.InternshipSpecialityId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error creating topic for speciality {model.InternshipSpecialityId}");
            TempData["ErrorMessage"] = "Failed to create the topic.";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reorder([FromBody] ReorderRequest request)
    {
        if (request == null || request.TopicIds == null)
            return BadRequest("Invalid data.");

        try
        {
            await specialityService.ReorderTopicsAsync(request.SpecialityId, request.TopicIds);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error reordering topics for speciality {request.SpecialityId}");
            return BadRequest("Could not reorder topics.");
        }
    }
    public class ReorderRequest
    {
        public Guid SpecialityId { get; set; }
        public List<Guid> TopicIds { get; set; }
    }

}
