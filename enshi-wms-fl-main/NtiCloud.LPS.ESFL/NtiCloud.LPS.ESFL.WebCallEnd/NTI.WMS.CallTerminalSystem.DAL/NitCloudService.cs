using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;

namespace NTI.WMS.CallTerminalSystem.DAL
{
    public class NitCloudService
    {
        public static string PostGatewayOld(string url, string postData, string token = null)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
                // 这里设置了协议类型。
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
                request.KeepAlive = false;
                //ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            request.Method = "POST"; //使用POST方式发送数据
            //request.Method = "GET"; //使用POST方式发送数据
            request.ContentType = "application/json;charset=utf-8";
            request.AllowAutoRedirect = true;
            //request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {token}");
            //request.Headers.Add("Authorization", $"{token}");

            //request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 dingtalk-win/1.0.0 nw(0.14.7) DingTalk(4.5.3-Release.22) Mojo/1.0.0 Native AppType(release)";
            //request.Accept = "*/*";
            //request.ContentLength = 2000;
            request.SendChunked = true;

            byte[] data = Encoding.UTF8.GetBytes(postData);
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            //获取网页响应结果
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            string result = string.Empty;
            using (StreamReader sr = new StreamReader(stream))
            {
                result = sr.ReadToEnd();
            }
            JObject Entity = JObject.Parse(result);
            string res = Convert.ToString(Entity["Entity"] ?? "");
            if (string.IsNullOrEmpty(res))
            {
                throw new Exception(Convert.ToString(Entity["Message"] ?? ""));
            }
            return res;
        }

        /// <summary>
        /// 调用微服务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        public static string PostGateway(string url, string postData, string tokenValue = null)
        {
            WriteLog.WriterSystemLogs($"url={url},postData={postData}", true);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            if (tokenValue != null)
            {
                wc.Headers.Add("Authorization", $"{tokenValue}");
            }

            wc.Headers.Add("Content-Type", "application/json;charset=utf-8");
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            try
            {
                string result = wc.UploadString(url, postData);
                return result;
            }
            catch (Exception ex)
            {
                WriteLog.WriterErrorLogs(ex.Message);
                throw new Exception(ex.Message);
            }
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
            //WriteLog.WriterSystemLogs($"url={url},sql={sql}", true);
            string errMsg = string.Empty;
            DataTable dt = new DataTable();
            url += "api/platform/Database/SelectSql";
            string dbJson = "{\"sql\":\"" + sql + "\",\"parameters\":[],\"queryStr\": \"\",  \"isEncrypt\": false}";
            string sqlDataTable = NitCloudService.PostGateway(url, dbJson, token);
            JObject entity = JObject.Parse(sqlDataTable);
            if (!Convert.ToBoolean(entity["isSucceed"].ToString()))
            {
                throw new Exception(entity["json"].ToString());
            }
            if (entity["json"].ToString() == "[]")
            {
                return dt;
                //throw new Exception($"获取数据为空，url={url}，sessionKey={sessionKey}，sql={sql}，token={token}，查询数据结果:{sqlDataTable}");
            }
            dt = JsonConvert.DeserializeObject<DataTable>(entity["json"].ToString());
            if (dt == null)
            {
                errMsg = $"获取数据失败，url={url}，sessionKey={sessionKey}，sql={sql}，token={token}，查询数据结果:{sqlDataTable}";
                WriteLog.WriterErrorLogs(errMsg);
                throw new Exception(errMsg);
            }
            if (dt.Rows.Count > 0 && dt.Columns.Contains("error"))
            {
                errMsg = $"查询数据反馈的错误信息：{dt.Rows[0]["error"]}，参数：url={url},sessionKey={sessionKey},sql={sql},token={token}";
                WriteLog.WriterErrorLogs(errMsg);
                throw new Exception(dt.Rows[0]["error"].ToString());
            }
            return dt;
        }
    }
}
