using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NtiCloud.Web.Common
{
    public class HttpClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HttpClient> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        public HttpClient(ILogger<HttpClient> logger, IHttpClientFactory clientFactory, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _serviceProvider = serviceProvider;
            this._configuration = configuration;
        }

        public async Task<string> HttpClientPostData(string action, string paramStr, Dictionary<string, string> token = null)
        {
            var t1 = DateTime.Now;
            var guid = t1.GetHashCode().ToString(); //Guid.NewGuid().ToString("N");
            _logger.LogInformation($@"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | ApiGateway 开始线程{guid}访问Url：{action}，参数：{paramStr}");
            try
            {
                if (!_configuration.GetSection("GatewayUrl").Value.StartsWith("http"))
                {
                    action = $@"http{(_configuration.GetSection("WebHttpsPorts").Get<IEnumerable<string>>()?.ToArray().Length > 0?"s":"")}://{HttpContext.Current.Request.Headers["Host"].ToString()}/api/{action}";
                }
                var client = _clientFactory.CreateClient("WebGatewayNtiCloud");

                client.DefaultRequestHeaders.Add("Accept-Language", "zh-cn");
                if (token != null)
                {
                    foreach (KeyValuePair<string, string> headerKeyValue in token)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(headerKeyValue.Key, headerKeyValue.Value);
                    }
                }
                HttpResponseMessage response;
                using (HttpContent httpContent = new StringContent(paramStr, Encoding.UTF8))
                {
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");//application/json
                    response = await client.PostAsync(action, httpContent);
                }
                if (response != null && response.IsSuccessStatusCode)
                {
                    var contentType = response.Content.Headers.ContentType;
                    if (contentType != null && string.IsNullOrEmpty(contentType?.CharSet))
                        contentType.CharSet = "utf-8";
                    string resp = await response.Content.ReadAsStringAsync();
                    return resp;
                }
                else
                {
                    return response.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($@"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | ApiGateway 出错线程{guid}请求Url：{action},{ex.Message.ToString()}");
                return ex.Message.ToString();
            }
            finally
            {
                var t2 = DateTime.Now;
                _logger.LogInformation($@"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | ApiGateway 完成线程{guid}请求Url：{action},耗时{(t2 - t1).TotalMilliseconds}毫秒");
            }
        }

        public Tuple<string, Dictionary<string, string>> GetWebToken()
        {
            string ntiToken = HttpContext.Current.Session.GetString(".Nti56.Com.Session");
            string tokenAppSecret = "noLogin";
            if (string.IsNullOrEmpty(ntiToken))
            {
                return Tuple.Create(tokenAppSecret, new Dictionary<string, string>());
            }
            else
            {
                var plaintext = Encoding.UTF8.GetString(Convert.FromBase64String(ntiToken));
                Dictionary<string, string> userToken = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(plaintext);
                if (userToken.ContainsKey("tokenAppSecret"))
                {
                    tokenAppSecret = userToken["tokenAppSecret"];
                    userToken.Remove("tokenAppSecret");
                }
                return Tuple.Create(tokenAppSecret, userToken);
            }
        }
        public void SetWebToken(string plaintext)
        {
            var ciphertext = Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext));
            HttpContext.Current.Session.SetString(".Nti56.Com.Session", ciphertext);
        }
    }
}
