using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace NtiCloud.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            string[] webHttpPorts = configuration.GetSection("WebHttpPorts").Get<IEnumerable<string>>()?.ToArray();
            string[] webHttpsPorts = configuration.GetSection("WebHttpsPorts").Get<IEnumerable<string>>()?.ToArray();
            string CertificateFileName = configuration.GetSection("CertificateFileName").Value;
            string CertificatePassword = configuration.GetSection("CertificatePassword").Value;

            IHostBuilder iWebHost = Host.CreateDefaultBuilder(args)
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddEventSourceLogger();
                    })
                    .UseKestrel((context, options) =>//设置Kestrel服务器
                    {
                        for (int i = 0; i < webHttpPorts?.Length; i++)//Http
                        {
                            if (webHttpPorts[i].Trim() == "")
                            {
                                continue;
                            }
                            options.Listen(IPAddress.Any, Convert.ToInt32(webHttpPorts[i].Trim()));
                        }
                        for (int i = 0; CertificateFileName != "" && CertificatePassword != "" && i < webHttpsPorts?.Length; i++)//Https
                        {
                            if (webHttpsPorts[i].Trim() == "")
                            {
                                continue;
                            }

                            options.Listen(IPAddress.Any, Convert.ToInt32(webHttpsPorts[i].Trim()), listenOptions =>
                            {
                                //填入之前iis中生成的pfx文件路径和指定的密码　　　　　　　　　　　　
                                if (File.Exists(Path.Combine(AppContext.BaseDirectory, CertificateFileName)))
                                {
                                    listenOptions.UseHttps(CertificateFileName, CertificatePassword);
                                }
                                else
                                {
                                    listenOptions.UseHttps();
                                }
                            });
                        }
                    })
                    .UseKestrel(opt => 
                    { 
                        opt.Limits.MinRequestBodyDataRate = null;
                        opt.Limits.MaxRequestBodySize = long.MaxValue;
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            return iWebHost;
        }
    }
}