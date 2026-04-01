using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace NtiCloud.Web.Common
{
    public class RouteTransformer : DynamicRouteValueTransformer
    {
        public override async ValueTask<RouteValueDictionary> TransformAsync(Microsoft.AspNetCore.Http.HttpContext httpContext, RouteValueDictionary values)
        {
            if (values == null)
            {
                values = new RouteValueDictionary();
                string host = httpContext.Request.Host.Host;
                using (Stream jsonStream = new StreamReader("appsettings.json").BaseStream)
                {
                    using (JsonDocument document = await JsonDocument.ParseAsync(jsonStream))
                    {
                        foreach (JsonElement subdomain in document.RootElement.GetProperty("SubDomains").EnumerateArray())
                        {
                            string curdomain = subdomain.GetProperty("Domain").GetString();
                            if (curdomain == host)
                            {
                                values["area"] = subdomain.GetProperty("AreaName").GetString();
                                values["controller"] = subdomain.GetProperty("Controller").GetString();
                                values["action"] = subdomain.GetProperty("Action").GetString();
                                string id = subdomain.GetProperty("ID").GetString();
                                if (id != "")
                                {
                                    string[] suffix = id.Split("&");
                                    values["id"] = suffix[0];
                                    for (int n = 1; n < suffix.Length; n++)
                                    {
                                        values[suffix[n].Split('=')[0]] = suffix[n].Split('=')[1];
                                    }
                                }
                                return values;
                            }
                        }
                    }
                }
            }
            else if (values["action"] == null)
            {
                values["action"] = "Index";
            }
            return values;
        }
    }
}
