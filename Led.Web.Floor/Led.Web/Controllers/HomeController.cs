using Led.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Led.Web.Controllers
{
    public class HomeController : Controller
    {
        ILedService ledService;

        public HomeController(ILedService ledService)
        {
            this.ledService = ledService;
        }
        public ActionResult Index()
        {
            var itemProducts = ledService.GetTodayInBoundInfo();
            ViewBag.LastClassProduction = ledService.GetLastClassQty();
            return View("Index",itemProducts);
        }

      
    }
}