using Microsoft.AspNetCore.Mvc;

namespace NtiCloud.Web.Areas.Daily.Controllers
{
    [Area("Daily")]
    [Route("Daily/[controller]/[action]")]
    public class MainController : Controller
    {
        public IActionResult Template()
        {
            return View();
        }
        public IActionResult EditTemplate()
        {
            return View();
        }
    }
}