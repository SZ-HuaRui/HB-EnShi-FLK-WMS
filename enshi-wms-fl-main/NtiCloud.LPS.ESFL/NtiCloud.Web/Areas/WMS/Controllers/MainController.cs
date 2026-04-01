using Microsoft.AspNetCore.Mvc;

namespace NtiCloud.Web.Areas.WMS.Controllers
{
    [Area("WMS")]
    [Route("WMS/[controller]/[action]")]
    public class MainController : Controller
    {
        public IActionResult Location()
        {
            return View();
        }

        public IActionResult ContainerDetailInfo()
        {
            return View();
        }
    }
}