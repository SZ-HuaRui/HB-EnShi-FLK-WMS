using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Text.Json;

namespace NtiCloud.Web.Controllers
{
    public class Authentication : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            /*
            var areaName = filterContext.ActionDescriptor.RouteValues["area"] ?? "";
            var controllerName = filterContext.ActionDescriptor.RouteValues["controller"] ?? "";
            var actionName = filterContext.ActionDescriptor.RouteValues["action"] ?? "";
            */
            IActionResult actionResultMain = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Main", action = "Login" }));

            if (filterContext.HttpContext.Request.Cookies["sessionKey"] == null)
            {
                filterContext.Result = actionResultMain;
            }
            else
            {
                JsonDocument userLoginInfo = Common.HttpContext.GetUserInfo() ?? Common.HttpContext.GetUserInfo();//防止失败
                Guid g = Guid.Empty;
                if (userLoginInfo == null || (!Guid.TryParse(userLoginInfo.RootElement.GetProperty("sessionKey").GetString(), out g)))
                {
                    filterContext.HttpContext.Response.Cookies.Delete("sessionKey");
                    filterContext.HttpContext.Response.Cookies.Append("sessionKey", "noLogin");
                    filterContext.Result = actionResultMain;
                }
                else
                {
                    MemberInfo(userLoginInfo);
                }
            }
            base.OnActionExecuting(filterContext);
        }

        private void MemberInfo(JsonDocument userInfo)
        {
            string mySql = "", sqlServer = "";
            if (userInfo == null)
            {
                return;
            }

            if (userInfo.RootElement.GetProperty("userDbConnR").GetProperty("dbType").GetString() == "MySql" ||
                userInfo.RootElement.GetProperty("userDbConnR").GetProperty("dbType").GetString() == "PostgreSql")
            {
                mySql = " LIMIT 2";
                sqlServer = "";
            }
            else if (userInfo.RootElement.GetProperty("userDbConnR").GetProperty("dbType").GetString() == "SqlServer")
            {
                mySql = "";
                sqlServer = " top 2 ";
            }

            string dbMsg = "select " + sqlServer + " title, create_date from prod_system_version order by create_date desc " + mySql;
            string dbSql = "select " + sqlServer + " b.orderid,b.create_date,case when a.created_by=b.created_by then '0' else '1' end userqa from prod_workorder_header a,(select * from prod_workorder_detail where t_id in (select max(t_id) t_id from prod_workorder_detail group by orderid)) b where a.orderid=b.orderid and a.userid='" + userInfo.RootElement.GetProperty("userTid").GetString() + "' order by a.create_date desc " + mySql;
            if (userInfo.RootElement.GetProperty("userDbConnR").GetProperty("dbType").GetString() == "Oracle")
            {
                dbMsg = "select * from (select title, create_date from prod_system_version order by create_date desc) where rownum<=2 ";
                dbSql = "select * from (select b.orderid,b.create_date,case when a.created_by=b.created_by then '0' else '1' end userqa from prod_workorder_header a,(select * from prod_workorder_detail where t_id in (select max(t_id) t_id from prod_workorder_detail group by orderid)) b where a.orderid=b.orderid and a.userid='" + userInfo.RootElement.GetProperty("userTid").GetString() + "' order by a.create_date desc) where rownum<=2 ";
            }
            System.Data.DataSet dataSet = Common.HttpContext.GetDataSet(dbMsg + ";" + dbSql);
            ViewData["MsgWork"] = dataSet;
        }
    }
}