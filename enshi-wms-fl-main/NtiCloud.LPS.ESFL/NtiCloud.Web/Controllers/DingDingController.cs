using Microsoft.AspNetCore.Mvc;

namespace NtiCloud.Web.Controllers
{
    public class DingDingController : Controller
    {
        public IActionResult Interface(string id)
        {
            ViewBag.UserUrl = id;
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult FileList(string fileName, string fileSize, string fileType)
        {
            ViewBag.fileName = fileName;
            ViewBag.fileSize = fileSize;
            ViewBag.fileType = fileType;
            return View();
        }
    }
}