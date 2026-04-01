using Microsoft.AspNetCore.Mvc;

namespace NtiCloud.Web.Areas.RF.Controllers
{
    [Area("RF")]
    [Route("RF/[controller]/[action]")]
    public class LoginController : Controller
    {
        public IActionResult Index(string id)
        {
            ViewBag.UserUrl = id;
            return View();
        }
    }
}