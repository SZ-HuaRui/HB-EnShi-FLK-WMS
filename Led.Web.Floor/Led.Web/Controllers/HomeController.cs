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
        private readonly ILedService ledService;

        public HomeController(ILedService ledService)
        {
            this.ledService = ledService;
        }

        /// <summary>
        /// 首页页面加载
        /// </summary>
        public ActionResult Index()
        {
            // 页面只负责渲染框架，不带数据（数据交给 Ajax）
            return View();
        }

        [HttpGet]
        public JsonResult GetData(string startTime = null, string endTime = null)
        {
            try
            {
                DateTime? startDateTime = null;
                DateTime? endDateTime = null;

                // 如果提供了时间参数，尝试解析
                if (!string.IsNullOrEmpty(startTime))
                {
                    if (DateTime.TryParse(startTime, out DateTime parsedStartTime))
                    {
                        startDateTime = parsedStartTime;
                    }
                }

                if (!string.IsNullOrEmpty(endTime))
                {
                    if (DateTime.TryParse(endTime, out DateTime parsedEndTime))
                    {
                        endDateTime = parsedEndTime;
                    }
                }

                // 只有当两个时间都提供时才使用手动选择的时间，否则使用自动模式
                DateTime? queryStartTime = (startDateTime.HasValue && endDateTime.HasValue) ? startDateTime : null;
                DateTime? queryEndTime = (startDateTime.HasValue && endDateTime.HasValue) ? endDateTime : null;

                var itemProducts = ledService.GetTodayInBoundInfo(queryStartTime, queryEndTime) ?? new List<Led.Model.ItemProduction>();
                var lastClassProduction = ledService.GetLastClassQty(queryStartTime, queryEndTime);

                return Json(new
                {
                    ItemProducts = itemProducts,
                    LastClassProduction = lastClassProduction
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
