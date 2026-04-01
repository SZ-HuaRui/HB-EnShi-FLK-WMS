using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Text.Json;

namespace NtiCloud.Web.Areas.Admin.Controllers
{
    public class Authentication : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string areaName = filterContext.ActionDescriptor.RouteValues["area"] ?? "";
            string controllerName = filterContext.ActionDescriptor.RouteValues["controller"] ?? "";
            string actionName = filterContext.ActionDescriptor.RouteValues["action"] ?? "";

            string appCode = filterContext.HttpContext.Request.Query["AppCode"].ToString();
            string dbCode = filterContext.HttpContext.Request.Query["DbCode"].ToString();
            string clientId = filterContext.HttpContext.Request.Query["ClientId"].ToString();
            string[] wmsCode = new string[] { "WMS", "LYWMS", "CWMS", "IWMS", "SWMS", "PWMS", "FWMS","YWMS","LWMS", "PY", "LB" };
            if (wmsCode.Count(p => p == appCode) > 0)
            {
                actionName = "WMS";
            }
            else
            {
                actionName = "Index";
            }

            JsonDocument userInfo = Common.HttpContext.GetUserInfo() ?? Common.HttpContext.GetUserInfo();//防止失败
            if (userInfo == null ||
                userInfo.RootElement.EnumerateObject().FirstOrDefault(c => c.Name.Equals("sessionKey", StringComparison.OrdinalIgnoreCase)).Value.ValueKind == JsonValueKind.Undefined ||
                userInfo.RootElement.EnumerateObject().FirstOrDefault(c => c.Name.Equals("userName", StringComparison.OrdinalIgnoreCase)).Value.ValueKind == JsonValueKind.Undefined ||
                userInfo.RootElement.GetProperty("sessionKey").GetString() == "noLogin")
            {
                if (appCode == null || appCode == "")
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "", controller = "Main", action = "Login" }));
                }
                else if ((clientId == null || clientId == "") && (dbCode == null || dbCode == ""))
                {
                    filterContext.Result = new RedirectResult($@"/{areaName}/Login/{actionName}?AppCode={appCode}");
                }
                else if (clientId == null || clientId == "")
                {
                    filterContext.Result = new RedirectResult($@"/{areaName}/Login/{actionName}?AppCode={appCode}&DbCode={dbCode}");
                }
                else if (dbCode == null || dbCode == "")
                {
                    filterContext.Result = new RedirectResult($@"/{areaName}/Login/{actionName}/{clientId}?AppCode={appCode}");
                }
                else
                {
                    filterContext.Result = new RedirectResult($@"/{areaName}/Login/{actionName}/{clientId}?AppCode={appCode}&DbCode={dbCode}");
                }
            }
            else
            {
                ViewBag.UserName = userInfo.RootElement.GetProperty("userName").GetString();
            }
            base.OnActionExecuting(filterContext);
        }
    }
}