using Microsoft.AspNetCore.Mvc;

namespace NtiCloud.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class SystemController : Authentication
    {
        public IActionResult MenuList()
        {
            return View();
        }
        public IActionResult MobileMenu()
        {
            return View();
        }
        public IActionResult Table()
        {
            return View();
        }
        public IActionResult Personalinfo()
        {
            return View();
        }
        public IActionResult Permission()
        {
            return View();
        }
        public IActionResult TransFlow()
        {
            return View();
        }
        public IActionResult WebConfig(string id, string templateId)
        {
            ViewBag.formId = id;
            ViewBag.TemplateID = templateId;
            return View();
        }
        public IActionResult FormDesigner(string id)
        {
            ViewBag.formId = id;
            return View();
        }
        public IActionResult MicroServices()
        {
            return View();
        }
    }
}