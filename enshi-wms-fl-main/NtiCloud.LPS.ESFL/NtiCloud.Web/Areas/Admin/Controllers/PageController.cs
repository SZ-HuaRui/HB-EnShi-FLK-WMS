using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Text;
using Yahoo.Yui.Compressor;

namespace NtiCloud.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class PageController : Authentication
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        public PageController(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this._serviceProvider = serviceProvider;
            this._configuration = configuration;
        }

        private DataTable ReadForm(string formid)
        {
            var ClientId = HttpContext.Request.Query.ContainsKey("ClientId")? HttpContext.Request.Query["ClientId"].ToString():"";
            var AppCode = HttpContext.Request.Query.ContainsKey("AppCode") ?HttpContext.Request.Query["AppCode"].ToString():"";
            string sql = "select ifnull(b.modulejscript,a.modulejscript) modulejscript,ifnull(b.modulename,a.modulename) modulename,ifnull(b.modulehtml,a.modulehtml) modulehtml,ifnull(b.modulejson,a.modulejson) modulejson "
                + " from prod_app_module a left join prod_app_project_module b on (a.t_id=b.p_id and b.CLIENT_ID='"+ ClientId + "' and b.APPCODE='"+AppCode+"') where a.t_id='" + formid + "'";
            DataTable dataTable = Common.HttpContext.GetDataSet(sql).Tables[0];
#if !DEBUG
            if(!string.IsNullOrEmpty(dataTable.Rows[0]["modulejscript"].ToString()))
            {
                /*在线方线
                var modulejscript = _serviceProvider.GetService<HttpClient>()
                    .HttpClientPostData("https://tool.oschina.net/action/jscompress/js_compress?munge=1&linebreakpos=",
                    dataTable.Rows[0]["modulejscript"].ToString()).Result;
                JsonElement Entity = JsonDocument.Parse(modulejscript).RootElement;
                if (Entity.EnumerateObject().FirstOrDefault(c => c.Name.Equals("result", StringComparison.OrdinalIgnoreCase)).Value.ValueKind != JsonValueKind.Undefined)
                {
                    dataTable.Rows[0]["modulejscript"] = Entity.GetProperty("result").GetString();
                }*/
                JavaScriptCompressor compressor = new JavaScriptCompressor();
                compressor.Encoding = Encoding.UTF8;
                dataTable.Rows[0]["modulejscript"] = compressor.Compress(dataTable.Rows[0]["modulejscript"].ToString());
            }
#endif
            return dataTable;
        }
        public ActionResult PageForm(string id, string formid)
        {
            ViewBag.TemplateID = id;
            DataTable dataTable = ReadForm(formid);
            if (dataTable.Rows.Count > 0)
            {
                ViewBag.JsScript = dataTable.Rows[0]["modulejscript"];
            }
            return View();
        }
        public IActionResult PageList(string id, string formid)
        {
            ViewBag.TemplateID = id;
            DataTable dataTable = ReadForm(formid);
            if (dataTable.Rows.Count > 0)
            {
                ViewBag.JsScript = dataTable.Rows[0]["modulejscript"];
            }
            ViewBag.ReportUrl = _configuration.GetSection("ReportUrl").Value;
            return View();
        }
        public ActionResult PageFormTable(string id, string formid)
        {
            ViewBag.TemplateID = id;
            DataTable dataTable = ReadForm(formid);
            if (dataTable.Rows.Count > 0)
            {
                ViewBag.JsScript = dataTable.Rows[0]["modulejscript"];
            }
            return View();
        }
        public IActionResult PageWebPage(string formid)
        {
            DataTable dataTable = ReadForm(formid);
            if (dataTable.Rows.Count > 0)
            {
                ViewBag.JsScript = dataTable.Rows[0]["modulejscript"];
            }
            return View();
        }
        public ActionResult PageDetail(string formid)
        {
            DataTable dataTable = ReadForm(formid);
            if (dataTable.Rows.Count > 0)
            {
                ViewBag.JsScript = dataTable.Rows[0]["modulejscript"];
            }
            return View();
        }
        public IActionResult PageFree(string formid) //自由表单
        {
            DataTable dataTable = ReadForm(formid);
            if (dataTable.Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(dataTable.Rows[0]["modulehtml"].ToString()))
                {
                    dataTable.Rows[0]["modulehtml"] = "对不起，表单未发布！";
                }
                ViewBag.freeTitle = dataTable.Rows[0]["modulename"].ToString();
                ViewBag.formHtml = dataTable.Rows[0]["modulehtml"].ToString();
                ViewBag.JsScript = dataTable.Rows[0]["modulejscript"];
            }
            return View();
        }
        public ActionResult PageChart(string id, string formid)
        {
            ViewBag.TemplateID = id;
            DataTable dataTable = ReadForm(formid);
            if (dataTable.Rows.Count > 0)
            {
                ViewBag.JsScript = dataTable.Rows[0]["modulejscript"];
            }
            return View();
        }
        public ActionResult PageHelp()
        {
            return View();
        }
        public ActionResult UserCtrl()
        {
            return View();
        }
    }
}