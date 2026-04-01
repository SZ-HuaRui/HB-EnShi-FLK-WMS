using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Unicode;

namespace NtiCloud.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (Configuration.GetSection("Consul").GetValue<bool>("isRegister"))
            {
                services.AddHealthChecks();
            }

            services.AddSingleton<RouteService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ILoggerFactory, LoggerFactory>();

            services.AddControllers(options =>
            {
                options.Filters.Add<FilterController>();
                options.Filters.Add<ExceptionController>();
            }).AddControllersAsServices().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All);
                //options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
            });

            services.AddHttpClient("ApiNtiCloud", c =>
            {
                c.Timeout = TimeSpan.FromSeconds(300);//超时间时间设置为300秒
                c.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.6 Safari/537.36");
            }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12,
                ServerCertificateCustomValidationCallback = (x, y, z, m) => true,
                AutomaticDecompression = DecompressionMethods.GZip
            });

            //解决Multipart body length limit 134217728 exceeded
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services.AddCors(o =>
                o.AddPolicy("NtiCloud", builder => builder
                    .WithOrigins(Configuration.GetSection("Origins").Get<IEnumerable<string>>()?.ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowCredentials()
            ));

            NtiCloudAssembly._services = services;
            NtiCloudAssembly.CompileDll(NtiCloudAssembly._services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            Kernel.Core.HttpContext.app = app;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            foreach (var origin in Configuration.GetSection("Origins").Get<IEnumerable<string>>()?.ToArray())
            {
                webSocketOptions.AllowedOrigins.Add(origin);
            }
            SocketHandler.Map(app, webSocketOptions);

            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwaggerUI((o) =>
            {
                //o.SwaggerEndpoint($"/swagger/v1/swagger.json", "敏捷研发工具服务");
                o.SwaggerEndpoint($"/swagger/main", "敏捷研发工具服务");
                var microServices = NtiCloudAssembly.microServicesInfo.Select(t => t.NameSpace).ToList().Distinct();
                foreach (var microService in microServices)
                {
                    o.SwaggerEndpoint($@"/swagger/{microService}", microService);
                }
            });

            #region register this service
            // consul的http check只需要返回是200, 就认为服务可用; 这里必须把Degraded改为503
            if (Configuration.GetSection("Consul").GetValue<bool>("isRegister"))
            {
                app.UseHealthChecks("/Health", new HealthCheckOptions()
                {
                    ResultStatusCodes =
                    {
                        [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                        [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });
                app.RegisterToConsul(Configuration, lifetime);
            }
            #endregion

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("NtiCloud");
            // 启用认证中间件
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapDynamicControllerRoute<RouteService>("/");
                endpoints.MapDynamicControllerRoute<RouteService>("{controller}/{action}");
                endpoints.MapDynamicControllerRoute<RouteService>("api/{controller}/{action}");
                endpoints.MapDynamicControllerRoute<RouteService>("api/{area}/{controller}/{action}");
            });
        }
    }
}
