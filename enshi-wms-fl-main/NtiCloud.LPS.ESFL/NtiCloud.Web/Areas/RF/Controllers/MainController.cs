using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace NtiCloud.Web.Areas.RF.Controllers
{
    [Area("RF")]
    [Route("RF/[controller]/[action]")]
    public class MainController : NtiCloud.Web.Areas.Admin.Controllers.Authentication
    {

        public IActionResult Config()
        {
            return View();
        }
        public IActionResult RFMain(string AppCode, string id)
        {
            if (id == null || id == "")
            {
                id = "0";
            }
            string whereStr = " and a.p_id='" + id + "' ";
            string dbSql = "select a.t_id,a.p_id,a.appcode,a.rfname,a.rficon,a.isconfig,ifnull(b.rscount,0) rscount from prod_rf_config a "
                + "left join (select p_id,count(t_id) rscount from prod_rf_config group by p_id) b on a.t_id=b.p_id "
                + "where a.appcode='" + AppCode + "' " + whereStr + " order by sortno";
            DataTable dataTable = Common.HttpContext.GetDataSet(dbSql).Tables[0];
            ViewData["rfList"] = dataTable;
            return View();
        }
        public IActionResult RFPageList()
        {
            return View();
        }
    }
}