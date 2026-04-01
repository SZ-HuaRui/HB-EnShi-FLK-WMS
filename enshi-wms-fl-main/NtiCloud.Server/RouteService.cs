using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using NtiCloud.Kernel.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NtiCloud.Server
{
    public class RouteService : DynamicRouteValueTransformer
    {
        public override ValueTask<RouteValueDictionary> TransformAsync(Microsoft.AspNetCore.Http.HttpContext httpContext, RouteValueDictionary values)
        {
            if (httpContext.Request.Method.ToUpper() == "OPTIONS")
            {

            }
            else if (values == null)
            {
                values = new RouteValueDictionary();
                values["controller"] = "Service";
                values["action"] = "Index";
            }
            else if (values["controller"].ToString().Equals("Service", StringComparison.OrdinalIgnoreCase))
            { }
            else if (values["controller"].ToString().Equals("Swagger", StringComparison.OrdinalIgnoreCase))
            {
                values["action"] = "Main";
            }
            else
            {
                var path = httpContext.Request.Path.ToString();
                var query = httpContext.Request.QueryString.Value;
                MicroServicesInfo microServicesInfo = NtiCloudAssembly.microServicesInfo?
                    .Where(t => t.ApiUrl.Equals(path, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (microServicesInfo != null)
                {
                    bool isFile = false, isQuery = true;
                    foreach (var parameterInfo in microServicesInfo.InputParameter)
                    {
                        if (parameterInfo.ParameterType.FullName.IndexOf("Microsoft.AspNetCore.Http.IFormFile") != -1)
                        {
                            isFile = true;
                            break;
                        }
                        if (isQuery && (parameterInfo.ParameterType.Name == "String" ||
                            parameterInfo.ParameterType.Name == "Int32" ||
                            parameterInfo.ParameterType.Name == "DateTime" ||
                            parameterInfo.ParameterType.Name == "Double"))
                        {
                            isQuery = true;
                        }
                        else
                        {
                            isQuery = false;
                        }
                    }
                    if (isFile)
                    {
                        values["controller"] = "Service";
                        values["action"] = "File";
                    }
                    else if (httpContext.RequestAborted.IsCancellationRequested ||
                        (isQuery && string.IsNullOrEmpty(query) == false && microServicesInfo.InputParameter.Length > 0) ||
                        microServicesInfo.ApiMethod == "Get" || microServicesInfo.InputParameter.Length == 0)
                    {
                        values["controller"] = "Service";
                        values["action"] = "Query";
                    }
                    else if (httpContext.Request.ContentType.Contains("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                    {
                        values["controller"] = "Service";
                        values["action"] = "Form";
                    }
                    else
                    {
                        values["controller"] = "Service";
                        values["action"] = "Route";
                    }
                }
            }
            return new ValueTask<RouteValueDictionary>(values);
        }
    }
}
