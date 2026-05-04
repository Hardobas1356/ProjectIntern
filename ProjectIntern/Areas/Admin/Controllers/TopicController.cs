using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IActionResult> Edit(Guid topicId, Guid specialityId)
    {
        try
        {
            TopicEditInputModel model = await topicService.GetTopicForEdit(topicId, specialityId);

            return View(model);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, $"Error editing topic for speciality. Topic ({topicId}) or speciality ({specialityId}) do not exist");
            TempData["ErrorMessage"] = "Failed to edit the topic.";
            return RedirectToAction("Index", "Speciality");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error editing topic for speciality {specialityId}");
            TempData["ErrorMessage"] = "Failed to edit the topic.";
            return RedirectToAction("Details", "Speciality", new { id = specialityId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TopicEditInputModel model)
    {
        try
        {
            if (!ModelState.IsValid) return View(model);

            await topicService.EditTopicAsync(model);

            return RedirectToAction("Details", "Speciality", new { id = model.specialityId });
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, $"Error saving edited topic for speciality {model.specialityId}");
            TempData["ErrorMessage"] = "Failed to save the edited topic.";
            return View(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error editing topic for speciality {model.specialityId}");
            TempData["ErrorMessage"] = "Failed to edit the topic.";
            return View(model);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Create(Guid specialityId)
    {
        TopicCreateInputModel model = new TopicCreateInputModel { InternshipSpecialityId = specialityId };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SoftDeleteTopic(Guid topicId, Guid specialityId)
    {
        try
        {
            await topicService.SoftDeleteTopicAsync(topicId);
            TempData["SuccessMessage"] = "Topic has been successfully deleted.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error deleting topic {topicId}");
            TempData["ErrorMessage"] = "Failed to delete the topic.";
        }

        return RedirectToAction("Details", "Speciality", new {id = specialityId});
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> RestoreTopic(Guid topicId, Guid specialityId)
    {
        try
        {
            await topicService.RestoreTopicAsync(topicId);
            TempData["SuccessMessage"] = "Topic has been successfully restored.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error restoring topic {topicId}");
            TempData["ErrorMessage"] = "Failed to restore the topic.";
        }

        return RedirectToAction("Details", "Speciality", new { id = specialityId });
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
        public List<Guid>? TopicIds { get; set; }
    }

}
