using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;

namespace NtiCloud.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class MainController : Authentication
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        public MainController(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }
        public IActionResult Index(string AppCode, string DbCode, string ClientId)
        {
            JsonDocument userInfo = Common.HttpContext.GetUserInfo();
            string appPic = string.Empty;
            if (string.IsNullOrEmpty(ClientId)) ClientId = "";
            if (userInfo == null)
            {
                string dbUrl = "";
                if (DbCode != null && DbCode != "")
                {
                    dbUrl = "&DbCode=" + DbCode;
                }

                if (ClientId == "")
                {
                    return Redirect("/Admin/Login?AppCode=" + AppCode + dbUrl);
                }
                else
                {
                    return Redirect("/Admin/Login/Index/" + ClientId + "?AppCode=" + AppCode + dbUrl);
                }
            }
            if (AppCode != null)
            {
                DataSet website = Common.HttpContext.GetDataSet($@"select * from prod_config_website where APPCODE='{AppCode}' and CLIENT_ID='{ClientId}';select * from prod_config_website where APPCODE='{AppCode}' and CLIENT_ID='';select * from prod_app_product where APPCODE='{AppCode}'");
                if (website.Tables[0].Rows.Count != 0)
                {
                    string url = website.Tables[0].Rows[0]["apphomepage"].ToString();
                    if (url.IndexOf("?") != -1)
                    {
                        bool appcoudUrl = true, clientUrl = true;
                        string[] urlArr = url.Split('?'), linkArr = urlArr[1].Split('&');
                        url = urlArr[0];
                        for (int i = 0; i < linkArr.Length; i++)
                        {
                            if (linkArr[i].ToLower().IndexOf("appcode") == 0)
                            {
                                appcoudUrl = false;
                            }
                            else if (linkArr[i].ToLower().IndexOf("clientid") == 0)
                            {
                                clientUrl = false;
                            }
                            if (i == 0)
                            {
                                url = url + "?" + linkArr[i];
                            }
                            else
                            {
                                url = url + "&" + linkArr[i];
                            }
                        }
                        if (appcoudUrl) url = "&AppCode=" + AppCode;
                        if (clientUrl) url = "&ClientId=" + ClientId;
                    }
                    else
                    {
                        url = url + "?AppCode=" + AppCode + "&ClientId=" + ClientId;
                    }
                    ViewBag.APPNAME = website.Tables[0].Rows[0]["appname"];
                    ViewBag.DeskTopUrl = url;

                    if (website.Tables[0].Rows[0]["applogo"].ToString().Trim() != "")
                    {
                        appPic = website.Tables[0].Rows[0]["applogo"].ToString();
                    }
                    else
                    {
                        appPic = "/areas/admin/img/index_logo_" + AppCode.ToLower() + ".png";
                    }
                }
                else if (website.Tables[1].Rows.Count != 0)
                {
                    string url = website.Tables[1].Rows[0]["apphomepage"].ToString();
                    if (url.IndexOf("?") != -1)
                    {
                        bool appcoudUrl = true, clientUrl = true;
                        string[] urlArr = url.Split('?'), linkArr = urlArr[1].Split('&');
                        url = urlArr[0];
                        for (int i = 0; i < linkArr.Length; i++)
                        {
                            if (linkArr[i].ToLower().IndexOf("appcode") == 0)
                            {
                                appcoudUrl = false;
                            }
                            else if (linkArr[i].ToLower().IndexOf("clientid") == 0)
                            {
                                clientUrl = false;
                            }
                            if (i == 0)
                            {
                                url = url + "?" + linkArr[i];
                            }
                            else
                            {
                                url = url + "&" + linkArr[i];
                            }
                        }
                        if (appcoudUrl) url = "&AppCode=" + AppCode;
                        if (clientUrl) url = "&ClientId=" + ClientId;
                    }
                    else
                    {
                        url = url + "?AppCode=" + AppCode + "&ClientId=" + ClientId;
                    }
                    ViewBag.APPNAME = website.Tables[1].Rows[0]["appname"];
                    ViewBag.DeskTopUrl = url;
                    if (website.Tables[1].Rows[0]["applogo"].ToString().Trim() != "")
                    {
                        appPic = website.Tables[1].Rows[0]["applogo"].ToString();
                    }
                    else
                    {
                        appPic = "/areas/admin/img/index_logo_" + AppCode.ToLower() + ".png";
                    }
                }
                else if (HttpContext.Request.Host.Host == "ding.nti56.com")
                {
                    ViewBag.DeskTopUrl = "/Admin/System/Personalinfo";
                    appPic = "/areas/admin/img/index_logo_" + AppCode.ToLower() + ".png";
                }
                else
                {
                    if (website.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.APPNAME = website.Tables[2].Rows[0]["appname"];
                    }
                    ViewBag.DeskTopUrl = "/Admin/Main/Desktop?AppCode=" + AppCode + "&ClientId=" + ClientId;
                    appPic = "/areas/admin/img/index_logo_" + AppCode.ToLower() + ".png";
                }
            }
            //系统名称
            ViewBag.SystemName = _configuration.GetSection("SystemName").Value;
            string dbJson = "{\"num\":4,\"dbCode\":\"" + DbCode + "\"}";
            Dictionary<string, string> token = new Dictionary<string, string>
            {
                { "sessionKey", userInfo.RootElement.GetProperty("sessionKey").GetString() },
                { "appCode", AppCode },
                { "clientId", (ClientId ?? "") },
                { "localLanguage", (Common.HttpContext.Current.Request.Cookies["localLanguage"] ?? "zh-CN") }
            };
            string initializationJson = _serviceProvider.GetService<NtiCloud.Web.Controllers.GatewayController>().PostData("platform/system/initialization", dbJson, token);
            JsonElement Entity = JsonDocument.Parse(initializationJson).RootElement;
            if (Entity.GetProperty("isSucceed").GetBoolean() == true && Entity.GetProperty("errcode").GetInt16() == 0)
            {
                ViewBag.Encrypt = Entity.GetProperty("json").GetProperty("encrypt").GetString();
            }
            var NticloudCSS = NtiCloud.Web.Common.HttpContext.Current.Request.Cookies["NticloudCSS"]?.ToLower();
            if (NticloudCSS!=null &&(
                (NticloudCSS.Contains("coolblack") && NticloudCSS.Contains("coolblacklight") == false)
                || NticloudCSS.Contains("redlight") || NticloudCSS.Contains("greenlight") || NticloudCSS.Contains("purplelight") || 
                (NticloudCSS.Contains("blue") && NticloudCSS.Contains("bluelight")==false) || NticloudCSS.Contains("yellowlight")))
            {
                ViewBag.AppPic = appPic.Replace("/areas/admin/img/", "/areas/admin/img/light/");
            }
            else
            {
                ViewBag.AppPic = appPic.Replace("/areas/admin/img/", "/areas/admin/img/dark/");
            }
            return View();
        }
        public IActionResult WMS(string AppCode, string DbCode, string ClientId)
        {
            return Index(AppCode, DbCode, ClientId);
        }
        public IActionResult Desktop(string AppCode, string ClientId)
        {
            DataTable desktop = Common.HttpContext.GetDataSet("select T_ID,MODULENAME,MODULESQL,ACT_STATUS from prod_desktop_header where APPCODE='" + AppCode + "' and CLIENT_ID='" + ClientId + "' order by T_ID").Tables[0];
            if (desktop.Rows.Count == 0)
            {
                JsonDocument userInfo = Common.HttpContext.GetUserInfo();
                string dbJson = "{\"sql\":\"INSERT INTO prod_desktop_header (CLIENT_ID,t_id,ORG_ID,appcode,modulename,act_status,CREATED_BY,MODIFY_IP,CREATE_DATE) VALUES ('&CLIENT_ID&',@t_id,'','&AppCode&',@modulename,'1',@created_by, @modify_ip,now())\"," +
                    "\"json\":[{\"t_id\":\"\",\"modulename\":\"最新动态\"}]," +
                    "\"isEncrypt\":false,\"commit\":true}";
                Dictionary<string, string> token = new Dictionary<string, string>
                {
                    { "sessionKey", userInfo.RootElement.GetProperty("sessionKey").GetString() },
                    { "appCode", AppCode },
                    { "clientId", (ClientId ?? "") },
                    { "localLanguage", (Common.HttpContext.Current.Request.Cookies["localLanguage"] ?? "zh-CN") }
                };
                string desktopInsert = _serviceProvider.GetService<NtiCloud.Web.Controllers.GatewayController>().PostData("platform/database/updatesql", dbJson, token);
                ViewBag.DeskTop = Common.HttpContext.GetDataSet("select T_ID,MODULENAME,MODULESQL,ACT_STATUS from prod_desktop_header where APPCODE='" + AppCode + "' and CLIENT_ID='" + ClientId + "' order by T_ID").Tables[0];
            }
            else
            {
                desktop.DefaultView.RowFilter = "act_status='1'";
                ViewBag.DeskTop = desktop.DefaultView.ToTable();
            }
            return View();
        }
        public IActionResult Deskpage(string AppCode, string ClientId)
        {
            return Desktop(AppCode, ClientId);
        }
    }
}