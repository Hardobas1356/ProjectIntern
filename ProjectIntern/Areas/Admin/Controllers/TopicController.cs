using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.Topic;

namespace ProjectIntern.Areas.Admin.Controllers;

public class TopicController : BaseAdminController
{
    private readonly ITopicService topicService;
    private readonly ILogger<TopicController> logger;

    public TopicController(ITopicService topicService, ILogger<TopicController> logger)
    {
        this.topicService = topicService;
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

}
