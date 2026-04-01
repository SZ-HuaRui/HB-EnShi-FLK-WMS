using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NtiCloud.Web.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;

namespace NtiCloud.Web.Controllers
{
    public class MemberController : Authentication
    {
        private readonly IServiceProvider _serviceProvider;
        public MemberController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IActionResult ControlDesk()
        {
            JsonDocument userInfo = Common.HttpContext.GetUserInfo();
            if (userInfo == null)
            {
                return Redirect("/Main/Login");
            }
            string dbSql = "select a.categorycode,a.categoryname,a.sortno from prod_app_category a , prod_app_category_contain b,prod_permission_app c "
                + "where a.categorycode = b.categorycode and b.APPCODE = c.APPCODE and c.userid='" + userInfo.RootElement.GetProperty("userTid").GetString() + "' and c.client_id='' order by a.sortno";
            dbSql += ";select a.appcode,a.appname,ifnull(d.DESCRIPTION,a.appcolor) appcolor,a.appicon,a.applink,a.appnote,a.description,b.categorycode "
                + "from prod_app_product a, prod_app_category_contain b,prod_permission_app c, "
                + "(select dict_code,dict_name,DESCRIPTION from prod_system_dictionary where P_id='1059998107359645696') d "
                + "where a.p_id='0' and a.appcode = b.appcode and b.APPCODE = c.APPCODE and c.userid='" + userInfo.RootElement.GetProperty("userTid").GetString() + "' and c.client_id='' and d.dict_code=a.appcolor order by b.sortno";
            dbSql += ";select a.t_id,a.p_id,b.appcode,a.MODULENAME,a.DESCRIPTION from prod_app_module a,prod_app_product_module b "
                + "where a.MODULECODE = b.MODULECODE order by b.appcode,b.sortno";
            dbSql += ";select t_id,client_code,client_name from prod_common_project where t_id in (select CLIENT_ID from prod_common_org_user where USERTID='" + userInfo.RootElement.GetProperty("userTid").GetString() + "')";
            dbSql += ";select a.projectid,a.appcode,b.appcolor,b.appicon,a.projectappcode,a.projectappname,b.applink,a.projectdb,a.sortno from prod_common_project_app a,"
                + "(select m.appcode, ifnull(n.DESCRIPTION,m.appcolor) appcolor,m.appicon,m.applink from prod_app_product m, "
                + "(select dict_code, dict_name, DESCRIPTION from prod_system_dictionary where P_id = '1059998107359645696') n "
                + "where m.appcolor = n.dict_code) b,prod_permission_app c where a.appcode = b.appcode and a.APPCODE = c.APPCODE and "
                + "c.userid='" + userInfo.RootElement.GetProperty("userTid").GetString() + "' and a.PROJECTID in (select CLIENT_ID from prod_common_org_user where USERTID='" + userInfo.RootElement.GetProperty("userTid").GetString() + "') "
                + "order by a.sortno";

            DataSet dataSet = Common.HttpContext.GetDataSet(dbSql);
            if (dataSet.Tables.Count >= 5)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    ViewData["ProdType"] = dataSet.Tables[0].DefaultView.ToTable(true, "categorycode", "categoryname", "sortno");
                }
                else
                {
                    ViewData["ProdType"] = dataSet.Tables[0];
                }
                if (dataSet.Tables[1].Rows.Count > 0)
                {
                    ViewData["ProdApp"] = dataSet.Tables[1].DefaultView.ToTable(true, "appcode", "appname", "appcolor", "appicon", "applink", "appnote", "description", "categorycode");
                }
                else
                {
                    ViewData["ProdApp"] = dataSet.Tables[1];
                }
                if (dataSet.Tables[2].Rows.Count > 0)
                {
                    ViewData["ProdModule"] = dataSet.Tables[2].DefaultView.ToTable(true, "appcode", "modulename", "description");
                }
                else
                {
                    ViewData["ProdModule"] = dataSet.Tables[2];
                }
                if (dataSet.Tables[3].Rows.Count > 0)
                {
                    ViewData["Project"] = dataSet.Tables[3].DefaultView.ToTable(true, "t_id", "client_code", "client_name");
                }
                else
                {
                    ViewData["Project"] = dataSet.Tables[3];
                }
                if (dataSet.Tables[4].Rows.Count > 0)
                {
                    ViewData["ProjApp"] = dataSet.Tables[4].DefaultView.ToTable(true, "projectid", "appcode", "appcolor", "appicon", "projectappcode", "projectappname", "applink", "projectdb", "sortno");
                }
                else
                {
                    ViewData["ProjApp"] = dataSet.Tables[4];
                }
            }
            else
            {
                ViewData["ProdType"] = new DataTable();
                ViewData["ProdApp"] = new DataTable();
                ViewData["ProdModule"] = new DataTable();
                ViewData["Project"] = new DataTable();
                ViewData["ProjApp"] = new DataTable();
            }
            return View();
        }
        public IActionResult AI()
        {
            JsonDocument userInfo = Common.HttpContext.GetUserInfo();
            if (userInfo == null)
            {
                return Redirect("/Main/Login");
            }
            string dbSql = "select a.appcode,a.appname,ifnull(b.DESCRIPTION,a.appcolor) appcolor,a.appimage,a.appicon,a.applink,a.appnote "
                + "from prod_app_product a,(select dict_code,dict_name,DESCRIPTION from prod_system_dictionary where P_id='1059998107359645696') b ,prod_permission_app c "
                + "where a.p_id='1101037030919507969' and c.userid='" + userInfo.RootElement.GetProperty("userTid").GetString() + "' and a.appcode=c.appcode and b.dict_code=a.appcolor order by a.t_id";
            ViewData["ProdList"] = Common.HttpContext.GetDataSet(dbSql).Tables[0];
            return View();
        }
        public IActionResult ProdList(string prodcode)
        {
            JsonDocument userInfo = Common.HttpContext.GetUserInfo();
            if (userInfo == null)
            {
                return Redirect("/Main/Login");
            }
            string prodSql = "select t_id,appname from prod_app_product where appcode='" + prodcode + "'";
            DataTable dataTable = Common.HttpContext.GetDataSet(prodSql).Tables[0];
            if (dataTable.Rows.Count > 0)
            {
                ViewBag.AppName = dataTable.Rows[0]["appname"];
                string dbSql = "select a.appcode,a.appname,ifnull(b.DESCRIPTION,a.appcolor) appcolor,a.appicon,a.applink,a.appnote "
                    + "from prod_app_product a,(select dict_code,dict_name,DESCRIPTION from prod_system_dictionary where P_id='1059998107359645696') b ,prod_permission_app c "
                    + "where a.p_id='" + dataTable.Rows[0]["t_id"] + "' and c.userid='" + userInfo.RootElement.GetProperty("userTid").GetString() + "' and a.appcode=c.appcode and b.dict_code=a.appcolor order by a.t_id";
                DataTable data_Table = Common.HttpContext.GetDataSet(dbSql).Tables[0];
                if (data_Table.Rows.Count > 0)
                {
                    ViewData["ProdList"] = data_Table.DefaultView.ToTable(true, "appcode", "appname", "appcolor", "appicon", "applink", "appnote");
                }
                else
                {
                    ViewData["ProdList"] = data_Table;
                }
            }
            else
            {
                ViewData["ProdList"] = new DataTable();
            }
            return View();
        }
        public IActionResult Help()
        {
            return View();
        }
        public IActionResult Message()
        {
            return View();
        }
        public IActionResult MyWorkOrder()
        {
            JsonDocument userInfo = Common.HttpContext.GetUserInfo();
            string dbSql = "select ifnull(count(t_id),0) allnum, ifnull(sum(case when WORKFLOW='3' or WORKFLOW='4' then 0 else 1 end),0) inprnum,ifnull(sum(case when WORKFLOW='3' or WORKFLOW='4' then 1 else 0 end),0) closenum from prod_workorder_header where userid='" + userInfo.RootElement.GetProperty("userTid").GetString() + "'";

            ViewData["MyWorkCount"] = Common.HttpContext.GetDataSet(dbSql).Tables[0];

            return View();
        }
        public IActionResult MyWorkOrderDetail(string id)
        {
            string dbSql = "select a.orderid,c.appNAME,d.dict_name QATYPE, b.workorder,b.create_date,b.created_by,case when a.created_by=b.created_by then '0' else '1' end userqa,e.dict_name WORKFLOW,a.WORKFLOW workflowid  from prod_workorder_header a, prod_workorder_detail b, "
                + "prod_app_product c,(select dict_code, dict_name from prod_system_dictionary where p_id = "
                + " (select t_id from prod_system_dictionary where dict_code = 'QATYPE')) d,"
                + "(select dict_code, dict_name from prod_system_dictionary where p_id = "
                + " (select t_id from prod_system_dictionary where dict_code = 'workOrder')) e "
                + "where a.PRODCODE = c.appCODE and d.dict_code=a.QATYPE and e.dict_code=a.WORKFLOW and a.orderid=b.orderid and a.orderid='" + id + "'";

            ViewData["workorder"] = Common.HttpContext.GetDataSet(dbSql).Tables[0];

            return View();
        }
        public IActionResult SubmitWorkOrder()
        {
            string dbSql = "select appcode,appname,appicon,appnote from prod_app_product";
            ViewData["prodlist"] = Common.HttpContext.GetDataSet(dbSql).Tables[0];

            string dbJson = "{\"dictionaryCodes\":\"QATYPE\"}";
            Dictionary<string, string> token = _serviceProvider.GetService<HttpClient>().GetWebToken().Item2;
            string dictJson = _serviceProvider.GetService<NtiCloud.Web.Controllers.GatewayController>().PostData("platform/system/dictionaryconfig", dbJson, token);

            ViewData["dictCode"] = JsonConvert.DeserializeObject<DataTable>(JsonDocument.Parse(dictJson).RootElement.GetProperty("json").GetString() ?? "[]");

            return View();
        }
    }
}