using Microsoft.AspNetCore.Mvc;

namespace ProjectIntern.Areas.Admin.Controllers;

public class AdminController : BaseAdminController
{
    public IActionResult Index()
    {
        return View();
    }
}