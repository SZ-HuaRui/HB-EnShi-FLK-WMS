using Microsoft.AspNetCore.Mvc;

namespace NtiCloud.Web.Areas.RFS.Controllers
{
    [Area("RFS")]
    public class LoginController : Controller
    {
        public ActionResult Index(string id)
        {
            ViewBag.Area = ControllerContext.ActionDescriptor.RouteValues["area"].ToString();
            ViewBag.clientId = id;
            ViewBag.UserIP = Common.HttpContext.GetUserIp();
            return View();
        }
    }
}