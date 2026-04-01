using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NtiCloud.Web.Models;
using System.Data;
using System.Diagnostics;


namespace NtiCloud.Web.Controllers
{
    public class MainController : Controller
    {
        private readonly ILogger<MainController> _logger;
        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            ViewBag.UserName = NtiCloud.Web.Common.HttpContext.RandomString();
            ViewBag.UserPswd = NtiCloud.Web.Common.HttpContext.RandomString();
            return View();
        }
        public IActionResult Partner()
        {
            return View();
        }
        public IActionResult Component()
        {
            return View();
        }
        public IActionResult Datastation()
        {
            return View();
        }
        public IActionResult IWMS()
        {
            return View();
        }
        public IActionResult Product()
        {
            string dbSql = "select categorycode,categoryname  from prod_app_category where isdisplay='1' order by sortno";
            dbSql += ";select a.appcode,appname,applink,appnote,APPIMAGE,categorycode from prod_app_product a,prod_app_category_contain b where a.appcode = b.appcode and a.ISDISPLAY = '1' order by b.sortno";
            dbSql += ";select b.appcode,a.MODULENAME,a.DESCRIPTION from prod_app_module a,prod_app_product_module b where a.MODULECODE = b.MODULECODE and b.ISSHOW='1' order by b.appcode,b.sortno";
            DataSet dataSet = Common.HttpContext.GetDataSet(dbSql);
            if (dataSet != null && dataSet.Tables.Count > 2)
            {
                ViewData["ProdType"] = dataSet.Tables[0];
                ViewData["ProdDT"] = dataSet.Tables[1];
                ViewData["ProdInfo"] = dataSet.Tables[2];
            }
            else
            {
                ViewData["ProdType"] = new DataTable();
                ViewData["ProdDT"] = new DataTable();
                ViewData["ProdInfo"] = new DataTable();
            }
            return View();
        }
        public IActionResult Project()
        {
            string dbSql = "SELECT t_id,industry,industryicon from prod_common_industry";
            dbSql += ";SELECT p_id,industrytext,industrynote from prod_common_industrys";

            DataSet dataSet = Common.HttpContext.GetDataSet(dbSql);
            if (dataSet != null && dataSet.Tables.Count > 1)
            {
                ViewData["ProdDT"] = dataSet.Tables[0];
                ViewData["ProdInfo"] = dataSet.Tables[1];
            }
            else
            {
                ViewData["ProdDT"] = new DataTable();
                ViewData["ProdInfo"] = new DataTable();
            }
            return View();
        }
        public IActionResult Register()
        {
            //ViewBag.UserIP = Common.HttpContext.GetUserIp();
            return View();
        }
        public IActionResult FindUserPswd()
        {
            return View();
        }
        public IActionResult Service()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}