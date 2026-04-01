using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWCS.Interface.WMSMicroService
{
    public class WMSMicroServiceUtil
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="sessionKey"></param>
        /// <returns>0表示远程服务器失败；1表示密码尝试超过限制，该用户已锁定；-1表示用户名或密码错误;2表示登录成功</returns>
        public static ResultObject WorkShop_Loging(string url, string userName, string password, string ip, out string sessionKey)
        {
            ResultObject resultObject = new ResultObject();
            try
            {
                url += "/api/platform/login/getlogin";
                JObject jObject = new JObject();
                jObject.Add("username", userName);
                jObject.Add("userpswd", Encryptcs.EncryptByAES(password));
                jObject.Add("islogin", "true");
                sessionKey = "";
                resultObject = PostGateway(url,jObject.ToString());
                if (!resultObject.IsSuccess)
                {
                    return resultObject;
                }
                JObject entity = JObject.Parse(resultObject.ResultJson);
                resultObject.IsSuccess = Convert.ToBoolean(entity["isSucceed"].ToString());
                sessionKey = Convert.ToString(entity["json"]["guid"] ?? "");
                return resultObject;
            }
            catch (Exception ex)
            {
                sessionKey = "";
                resultObject.IsSuccess = false;
                resultObject.ErrorMessage = ex.Message;
                return resultObject;
            }
          
        }



        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static ResultObject WorkShop_GetToken(string url, string sessionKey, out string token)
        {
            ResultObject resultObject = new ResultObject();
            try
            {
                url += "/api/oauth2/token";
                JObject jObject = new JObject();
                jObject.Add("sessionKey", sessionKey);
                jObject.Add("appCode", "WMS");
                jObject.Add("warehouseId", "1016319470013779968");
                jObject.Add("clientId", "");
                resultObject = PostGateway(url, jObject.ToString());
                token = "";
                if (!resultObject.IsSuccess)
                {
                    return resultObject;
                }
                JObject entity = JObject.Parse(resultObject.ResultJson);
                resultObject.IsSuccess = Convert.ToBoolean(entity["isSucceed"].ToString());
                token = Convert.ToString(entity["json"] ?? "");
                return resultObject;
            }
            catch(Exception ex)
            {
                token = " ";
                resultObject.IsSuccess = false;
                resultObject.ErrorMessage = ex.Message;
                return resultObject;
            }
        }


        /// <summary>
        /// 调用微服务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        public static ResultObject PostGateway(string url, string postData, string tokenValue = null)
        {
            WebClientProxy wc = new WebClientProxy();
            ResultObject resultObject = new ResultObject();
            wc.Encoding = Encoding.UTF8;
            if (tokenValue != null)
            {
                wc.Headers.Add("Authorization", $"{tokenValue}");
            }
            wc.Headers.Add("Content-Type", "application/json;charset=utf-8");
            wc.Headers.Add("Accept", "application/json;charset=utf-8");
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            try
            {
                JObject obj = JObject.Parse(wc.UploadString(url, postData));
                resultObject.ResultJson = obj.ToString();
                resultObject.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultObject.ErrorMessage = ex.Message;
            }
            return resultObject;
        }

      



        /// <summary>
        /// 拼接调用微服务需要的参数，并将返回值转化成DataTable
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="sql"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string url, string sessionKey, string sql, string token = null)
        {
            ResultObject resultObject = new ResultObject();
            DataTable dt = new DataTable();
            url += "api/platform/Database/SelectSql";
            string dbJson = "{\"sql\":\"" + sql + "\",\"parameters\":[],\"queryStr\": \"\",  \"isEncrypt\": false}";
            resultObject = PostGateway(url, dbJson, token);
            JObject entity = JObject.Parse(resultObject.ResultJson);
            if (entity["json"].ToString() == "[]")
            {
                return dt;
            }
            dt = JsonConvert.DeserializeObject<DataTable>(entity["json"].ToString());
            if (dt == null)
            {
                throw new Exception($"获取数据失败，url={url}，sessionKey={sessionKey}，sql={sql}，token={token}，查询数据结果:{resultObject.ResultJson}");
            }
            if (dt.Rows.Count > 0 && dt.Columns.Contains("error"))
            {
                throw new Exception($"查询数据反馈的错误信息：{dt.Rows[0]["error"]}，参数：url={url},sessionKey={sessionKey},sql={sql},token={token}");
            }
            return dt;
        }


        //客户端发出要料呼叫         
        public static string ClientCallPallet(string url, string sessionKey, string warehouseId, string dockNumber,
            string hasLGV, string runsType, string token = null)
        {
            ResultObject resultObject = new ResultObject();
            url += "api/wms/MachineMaterialService/CreateOutBoundByMachineDock";
            JObject jo = new JObject();
            jo.Add("machine", sessionKey);
            jo.Add("dock", warehouseId);
            jo.Add("productPlanOutId", dockNumber);
            jo.Add("shiftId", string.Empty);
            jo.Add("teamId", runsType);
            resultObject = PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(resultObject.ResultJson)) throw new Exception("要料调用微服务失败！");
            JObject entity = JObject.Parse(resultObject.ResultJson);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;  //成功
        }

        public static ResultObject RequestWMS(WMSServicRequest request)
        {
            ResultObject resultObject = new ResultObject();
            string sessionKey = string.Empty;
            string token = string.Empty;
            resultObject = WorkShop_Loging(request.BaseUrl, request.UserName, request.Password, request.Ip, out sessionKey);
            if (!resultObject.IsSuccess)
            {
                resultObject.ErrorMessage += ";登录WMS服务失败.";
                return resultObject;
            }
            if (string.IsNullOrEmpty(sessionKey))
            {
                resultObject.IsSuccess = false;
                resultObject.ErrorMessage += ";WMS反馈的SessionKey为空";
                return resultObject;
            }
            //resultObject = WorkShop_GetToken(request.BaseUrl, sessionKey, out token);
            //if (!resultObject.IsSuccess)
            //{
            //    resultObject.ErrorMessage += ";获取Token失败.";
            //    return resultObject;
            //}
            //20260310 llt 增加重试机制 
            int maxRetry = 5;
            int retryCount = 0;
            Random random = new Random();

            while (retryCount < maxRetry)
            {
                resultObject = WorkShop_GetToken(request.BaseUrl, sessionKey, out token);
                if (resultObject.IsSuccess && !string.IsNullOrEmpty(token))
                    break;

                retryCount++;
                if (retryCount >= maxRetry)
                {
                    resultObject.ErrorMessage += $";获取Token失败（已重试{maxRetry}次）";
                    return resultObject;
                }

                // 计算基础延迟：2^retryCount 秒（第1次重试等待2秒，第2次4秒，第3次8秒...）
                double baseDelay = Math.Pow(2, retryCount);
                // 添加抖动：0~1 秒
                double jitter = random.NextDouble() * 1.0;
                int delayMs = (int)((baseDelay + jitter) * 1000);

                Thread.Sleep(delayMs);
            }
            // 循环外检查 token 是否为空
            if (string.IsNullOrEmpty(token))
            {
                resultObject.IsSuccess = false;
                resultObject.ErrorMessage += ";WMS反馈的Token为空";
                return resultObject;
            }
            string postJson = JsonConvert.SerializeObject(request.PostData);
            try
            {
                resultObject = PostGateway(request.BaseUrl+request.DataUrl, postJson, token);
            }
            catch (Exception ex)
            {
                resultObject.IsSuccess = false;
                resultObject.ErrorMessage = ex.Message;
            }
            return resultObject;
        }
    }

    public class WMSServicRequest
    {
        public string BaseUrl { get; set; } = "http://10.157.168.166:9080";
        public string UserName { get; set; } = "wmsjk";
        public string Password { get; set; } = "Nti56.com";
        public string Ip { get; set; } = "";
        public object PostData { get; set; }
        public string DataUrl { get; set; }
       
    }

    public class ResultObject
    {
        public bool IsSuccess { get; set; } = false;
        public string ResultJson { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
