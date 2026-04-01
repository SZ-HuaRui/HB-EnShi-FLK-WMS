using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NtiCloud.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;


namespace NtiCloud.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class GatewayController : Controller
    {
        private readonly ILogger<GatewayController> _logger;
        private readonly IServiceProvider _serviceProvider;
        public GatewayController(ILogger<GatewayController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        private void WriteLog(string serviceName, string postData, string msg)
        {
            _logger.LogInformation($@"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | WriteLog 服务名：{serviceName}，参数：{postData}，错误信息{msg}");
            if (serviceName == "oauth2/token" || serviceName == "platform/user/gettoken")
            {
                return;
            }

            string service_Name = "platform/database/updatesql";

            string post_Data = JsonSerializer.Serialize(
                new
                {
                    sql = $@"insert into prod_system_log(t_id,client_id,org_id,log_type,log_attribute,log_titile,log_note,create_date,created_by,modify_ip)
                                                values(@t_id,@client_id,@org_id,@log_type,@log_attribute,@log_titile,@log_note,now(),@created_by,@modify_ip)",
                    json = new object[] {
                    new
                    {
                        t_id="",
                        client_id="",
                        org_id="",
                        log_type="ERROR",
                        log_attribute="API",
                        log_titile="服务调用出错",
                        log_note="服务："+serviceName+"<br>参数："+postData+"<br>原因："+msg
                    }},
                    isEncrypt = false
                });

            Dictionary<string, string> token = _serviceProvider.GetService<HttpClient>().GetWebToken().Item2;
            if (service_Name != serviceName && post_Data != postData)
            {
                PostData(service_Name, post_Data, token);
            }
        }

        private void PostToken(Dictionary<string, string> token = null)
        {
            _logger.LogInformation($@"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | PostToken 获取Token信息{JsonSerializer.Serialize(token).ToString()}");
            if (Common.HttpContext.Current.Request.Cookies["warehouseId"] != null &&
                Common.HttpContext.Current.Request.Cookies["warehouseId"].ToString() != "" &&
                token.ContainsKey("warehouseId") == false)
            {
                token.Add("warehouseId", Common.HttpContext.Current.Request.Cookies["warehouseId"].ToString());
            }
            if (!token.ContainsKey("source"))
            {
                token.Add("source", "Web");
            }
            string tokenPostData = JsonSerializer.Serialize(token);
            var tokenAppSecret = PostApiGateway("oauth2/token", tokenPostData);
            if (string.IsNullOrEmpty(tokenAppSecret))
            {
                Common.HttpContext.Current.Session.Clear();
            }
            else
            {
                JsonElement Entity = JsonDocument.Parse(tokenAppSecret).RootElement;
                if (Entity.GetProperty("isSucceed").GetBoolean() == true && Entity.GetProperty("errcode").GetInt16() == 0)
                {
                    tokenAppSecret = Entity.GetProperty("json").GetString();
                    token.Add("tokenAppSecret", tokenAppSecret);
                    _serviceProvider.GetService<HttpClient>().SetWebToken(JsonSerializer.Serialize(token));
                }
                else
                {
                    Common.HttpContext.Current.Session.Clear();
                }
            }
        }

        //防止跨站请求伪造（XSRF/CSRF）攻击 
        [ValidateAntiForgeryToken]
        [HttpPost]
        public string PostData(string serviceName, string postData, Dictionary<string, string> token = null)
        {
            if (HttpContext != null)
            {
                StringValues stringValues;
                HttpContext.Request.Headers.TryGetValue("Referer", out stringValues);
                if (stringValues.ToString().IndexOf(HttpContext.Request.Host.Host) == -1)
                {
                    _logger.LogError($@"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | 来从{stringValues.ToString()}的非法请求！");
                    return null;
                }
            }
            var userip = Common.HttpContext.GetUserIp();
            _logger.LogInformation($@"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | IP:{userip},PostData服务名：{serviceName}，参数：{postData},Token：{JsonSerializer.Serialize(token).ToString()}");
            bool getToken = false;
            if (token != null && token.ContainsKey("sessionKey") && token["sessionKey"] != "noLogin" && token["sessionKey"] != "")//判断是否需要请求Token
            {
                Tuple<string, Dictionary<string, string>> getWebToken = _serviceProvider.GetService<HttpClient>().GetWebToken();
                if (getWebToken.Item1 == "noLogin")
                {
                    getToken = true;
                }
                if (getToken == false)
                {
                    foreach (KeyValuePair<string, string> item in token)
                    {
                        string value = string.Empty;
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            value = item.Value;
                        }

                        if (getWebToken.Item2.ContainsKey(item.Key) == false || getWebToken.Item2[item.Key] != value)
                        {
                            getToken = true;
                            break;
                        }
                    }
                }
                if (getToken)
                {
                    PostToken(token);
                }
            }
            if (token != null && token.ContainsKey("sessionKey") && token["sessionKey"] != "noLogin"
                && token["sessionKey"] != "" && _serviceProvider.GetService<HttpClient>().GetWebToken().Item1 == "noLogin")
            {
                return null;
            }
            return PostApiGateway(serviceName, postData, token);
        }
        private string PostApiGateway(string serviceName, string postData, Dictionary<string, string> token = null)
        {
            string NtiCloudEncryptKey = $@"NtiCloudEncryptKey={Common.HttpContext.EncryptByAES(postData)}";
            Tuple<string, Dictionary<string, string>> getWebToken = _serviceProvider.GetService<HttpClient>().GetWebToken();
            Dictionary<string, string> tokenAppSecret = null;
            if (token != null && getWebToken.Item1 != "noLogin")
            {
                tokenAppSecret = new Dictionary<string, string>
                {
                    {"Authorization",getWebToken.Item1 }
                };
            }
            var result = _serviceProvider
                        .GetService<NtiCloud.Web.Common.HttpClient>()
                        .HttpClientPostData(serviceName, NtiCloudEncryptKey, tokenAppSecret)
                        .GetAwaiter().GetResult();
            if (result == "" && serviceName == "platform/user/gettoken")
            {
                Common.HttpContext.Current.Session.Clear();
                return PostData(serviceName, postData, token);
            }
            else
            {
                JsonElement Entity = JsonDocument.Parse(result).RootElement;
                if (Entity.EnumerateObject().FirstOrDefault(c => c.Name.Equals("errcode", StringComparison.OrdinalIgnoreCase)).Value.ValueKind != JsonValueKind.Undefined &&
                        Entity.GetProperty("errcode").GetInt16() == 401 && serviceName != "oauth2/token" && token != null && token.ContainsKey("sessionKey") && token["sessionKey"] != "noLogin" && token["sessionKey"] != "")
                {
                    _logger.LogWarning($@"参数：{postData},Token：{JsonSerializer.Serialize(token).ToString()},输出内容：{result}");
                    Common.HttpContext.Current.Session.Clear();
                    return PostData(serviceName, postData, token);
                }
                else
                {
                    return result;
                }
            }
        }

    }
}