using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NtiCloud.Web.Common;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using UEditorNetCore;
using Yahoo.Yui.Compressor;

namespace NtiCloud.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddUEditorService();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDistributedMemoryCache();
            string SessionTimeOut = Configuration.GetSection("SessionTimeOut").Value ?? "30";
            services.AddSession(o =>
            {
                o.Cookie.HttpOnly = true;
                o.Cookie.Name = ".Nti56.Com.Session";
                o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                o.IdleTimeout = TimeSpan.FromMinutes(Convert.ToInt32(SessionTimeOut));
            });//注册服务

            services.AddAntiforgery(op =>
            {
                op.FormFieldName = "Web_Token_XSRF_CSRF";
                //op.Cookie=new CookieBuilder{ Domain = "nti56.com" };
                op.Cookie.Name = ".Nti56.Com.Token";
                op.Cookie.HttpOnly = true;
                op.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                op.HeaderName = "VerificationToken";//手动高亮,防止跨站请求伪造（XSRF/CSRF）攻击,使用 antiforgery 令牌　　
            });

            //services.AddControllers().AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            //});

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.AllowTrailingCommas = true;
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                });

            services.AddSingleton<RouteTransformer>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<NtiCloud.Web.Common.HttpClient>();
            services.AddSingleton<NtiCloud.Web.Controllers.GatewayController>();

            services.AddControllersWithViews();
             //.AddRazorRuntimeCompilation();

            services.AddHttpClient("WebGatewayNtiCloud", c =>
            {
                if (Configuration["GatewayUrl"].StartsWith("http"))
                {
                    c.BaseAddress = new Uri(Configuration["GatewayUrl"] + "api/");//地址从配置文件appsettings.json里取
                }
                c.Timeout = TimeSpan.FromSeconds(30);//超时间时间设置为30秒
                c.DefaultRequestHeaders.Add("Accept", "*/*");
                c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory");
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
                x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Common.HttpContext.app = app;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Main/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Main}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "areas", "areas",
                    pattern: "{area:exists}/{controller=Login}/{action=Index}/{id?}");

                endpoints.MapDynamicControllerRoute<RouteTransformer>("/");

                endpoints.MapDynamicControllerRoute<RouteTransformer>("{area}/{controller}");
                endpoints.MapDynamicControllerRoute<RouteTransformer>("{area}/{controller}/{action}/{id}");
            });
            Task.Run(() => 
            {
                DirectoryInfo dir = new DirectoryInfo(System.Environment.CurrentDirectory);
                if (!dir.Name.Equals("NtiCloud.Web", StringComparison.OrdinalIgnoreCase))
                {
                    FindFile(dir);
                }
            });
        }
        private void FindFile(DirectoryInfo di)
        {
            FileInfo[] fis = di.GetFiles();
            for (int i = 0; i < fis.Length; i++)
            {
                //Console.WriteLine("文件：" + fis[i].FullName);
                try
                {
                    string extension = Path.GetExtension(fis[i].FullName);
                    if (extension.Equals(".js", StringComparison.OrdinalIgnoreCase))
                    {
                        JavaScriptCompressor compressor = new JavaScriptCompressor();
                        compressor.Encoding = Encoding.UTF8;
                        string source = System.IO.File.ReadAllText(fis[i].FullName);
                        source = compressor.Compress(source);
                        System.IO.File.WriteAllText(fis[i].FullName, source);
                        //Console.WriteLine($@"JS文件发布{fis[i].FullName}成功");
                    }
                    else if (extension.Equals(".css", StringComparison.OrdinalIgnoreCase))
                    {
                        CssCompressor compressor = new CssCompressor();
                        string source = System.IO.File.ReadAllText(fis[i].FullName);
                        source = compressor.Compress(source);
                        System.IO.File.WriteAllText(fis[i].FullName, source);
                        //Console.WriteLine($@"CSS文件发布{fis[i].FullName}成功");
                    }
                    else
                    {
                        break;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($@"校验文件{fis[i].FullName}提示：{ex.Message}");
                }
            }

            DirectoryInfo[] dis = di.GetDirectories();
            for (int j = 0; j < dis.Length; j++)
            {
                //Console.WriteLine("目录：" + dis[j].FullName);
                FindFile(dis[j]);
            }
        }
    }

    public class DatetimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), out DateTime date))
                {
                    return date;
                }
            }
            return reader.GetDateTime();
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
