using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace NtiCloud.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index(string id, string AppCode)
        {
            if (AppCode != null)
            {
                ViewBag.AppCode = AppCode;
                ViewBag.AppPic = "/areas/admin/img/login_logo_" + AppCode.ToLower() + ".png";
                DataTable website = Common.HttpContext.GetDataSet("select * from prod_config_website where APPCODE='" + AppCode + "' and CLIENT_ID='" + id + "'").Tables[0];
                if (website.Rows.Count != 0)
                {
                    ViewBag.APPNAME = website.Rows[0]["appname"];
                    ViewBag.APPTITLE = website.Rows[0]["apptitle"];
                    ViewBag.APPNOTE = website.Rows[0]["appnote"];
                    if (website.Rows[0]["applogo"].ToString().Trim() != "")
                    {
                        ViewBag.AppPic = website.Rows[0]["applogo"].ToString();
                    }
                }
                else
                {
                    DataTable dataTable = Common.HttpContext.GetDataSet("select * from prod_app_product where APPCODE='" + AppCode + "'").Tables[0];
                    if (dataTable.Rows.Count > 0)
                    {
                        ViewBag.APPNAME = dataTable.Rows[0]["appname"];
                        ViewBag.APPTITLE = dataTable.Rows[0]["apptitle"];
                        ViewBag.APPNOTE = dataTable.Rows[0]["appnote"];
                    }
                }
            }
            //系统名称
            ViewBag.SystemName = _configuration.GetSection("SystemName").Value;
            //公司名称
            ViewBag.CompanyName = _configuration.GetSection("CompanyName").Value;
            //版权信息
            ViewBag.CopyRight = _configuration.GetSection("CopyRight").Value;

            ViewBag.ClientId = id;
            ViewBag.Area = ControllerContext.ActionDescriptor.RouteValues["area"].ToString();
            return View();
        }

        public IActionResult WMS(string id, string AppCode)
        {
            return Index(id, AppCode);
        }
    }
}