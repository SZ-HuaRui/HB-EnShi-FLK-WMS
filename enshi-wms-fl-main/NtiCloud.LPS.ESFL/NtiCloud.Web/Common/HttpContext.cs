using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NtiCloud.Web.Common
{
    public static class HttpContext
    {
        public static IApplicationBuilder app;
        static HttpContext()
        { }
        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                object factory = app.ApplicationServices.GetService(typeof(IHttpContextAccessor));
                return ((IHttpContextAccessor)factory).HttpContext;
            }
        }
        public static string RandomString(int len = 8)
        {
            string[] arr = new string[] { "2", "3", "4", "5", "6", "7", "a", "c", "d", "e", "f", "h", "i", "j", "k", "m", "n", "p", "r", "s", "t", "A", "C", "D", "E", "F", "G", "H", "J", "K", "M", "N", "P", "Q", "R", "S", "U", "V", "W", "X", "Y", "Z" };
            string noncestr = "";
            int randValue = -1;
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < len; i++)
            {
                randValue = rand.Next(0, arr.Length - 1);
                noncestr += arr[randValue];
            }
            return noncestr;
        }

        public static string EncryptByAES(string input, string key = "abcdefghijklmnopqrstuvwxyz012345", string iv = "9876543210abcdef")
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key.Substring(0, 32));
                aesAlg.IV = Encoding.UTF8.GetBytes(iv.Substring(0, 16));
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                        byte[] bytes = msEncrypt.ToArray();

                        StringBuilder sb = new StringBuilder(bytes.Length * 3);
                        foreach (byte b in bytes)
                        {
                            //16进制数字
                            sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                            //16进制数字之间以空格隔开
                            //sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
                        }
                        return sb.ToString().ToUpper();
                    }
                }
            }
        }
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetUserIp()
        {
            return HttpContext.Current.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? HttpContext.Current.Connection.RemoteIpAddress.ToString();
        }
        public static string GetSessionKey()
        {
            if (HttpContext.Current.Request.Cookies["sessionKey"] == null)
            {
                return "noLogin";
            }
            else
            {
                return HttpContext.Current.Request.Cookies["sessionKey"].ToString();
            }
        }
        public static JsonDocument GetUserInfo()
        {
            Dictionary<string, string> token = HttpContext.Current.RequestServices.GetService<HttpClient>().GetWebToken().Item2;
            if (token.Count == 0)
            {
                token.Add("sessionKey", Common.HttpContext.GetSessionKey());
            }
            if (token["sessionKey"] == "noLogin")
            {
                HttpContext.Current.Response.Cookies.Delete("sessionKey");
                HttpContext.Current.Response.Cookies.Append("sessionKey", "noLogin", new CookieOptions() { IsEssential = true });
                return null;
            }
            string dbString = HttpContext.Current.RequestServices.GetService<NtiCloud.Web.Controllers.GatewayController>().PostData("platform/user/gettoken", "{}", token);
            if (string.IsNullOrEmpty(dbString)) return null;
            JsonDocument userMode = JsonDocument.Parse(dbString);
            JsonDocument userInfo = JsonDocument.Parse(userMode.RootElement.GetProperty("userLoginInfo").ToString());
            if (userInfo.RootElement.GetProperty("sessionKey").GetString() == "noLogin")
            {
                HttpContext.Current.Response.Cookies.Delete("sessionKey");
                HttpContext.Current.Response.Cookies.Append("sessionKey", "noLogin", new CookieOptions() { IsEssential = true });
            }
            return userInfo;
        }

        public static DataSet GetDataSet(string sql)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            dataSet.Tables.Add(dataTable);

            string timestamp = Convert.ToString((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            string ecryptJson = "{\"pswd\":\"" + timestamp + "\",\"opt\":\"set\",\"key\":\"nti56com\"}";
            string encryptKey = HttpContext.Current.RequestServices.GetService<NtiCloud.Web.Controllers.GatewayController>().PostData("platform/user/decryptpassword", ecryptJson);
            JsonElement Entity = JsonDocument.Parse(encryptKey).RootElement;
            if (Entity.GetProperty("isSucceed").GetBoolean() == true && Entity.GetProperty("errcode").GetInt16() == 0)
            {
                encryptKey = Entity.GetProperty("json").GetString();
                string dbJson = "{\"sql\":\"" + sql + "\",\"encryptKey\":\"" + encryptKey + "\"}";
                string sqlDataSet = HttpContext.Current.RequestServices.GetService<NtiCloud.Web.Controllers.GatewayController>().PostData("website/selectdataset", dbJson);
                Entity = JsonDocument.Parse(sqlDataSet).RootElement;
                if (Entity.GetProperty("isSucceed").GetBoolean() == true && Entity.GetProperty("errcode").GetInt16() == 0)
                {
                    sqlDataSet = Entity.GetProperty("json").GetString();
                    if (sqlDataSet == null || sqlDataSet == "[]" || sqlDataSet == "Error")
                    {
                        return dataSet;
                    }
                    return JsonConvert.DeserializeObject<DataSet>(sqlDataSet);
                }
            }
            return dataSet;
        }
    }
}
