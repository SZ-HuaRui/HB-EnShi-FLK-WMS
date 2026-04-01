using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace NtiCloud.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //捕获Ctrl+C事件
            Console.CancelKeyPress += Console_CancelKeyPress;
            //进程退出事件
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            //卸载事件
            //AssemblyLoadContext.Default.Unloading += Default_Unloading;

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string[] webHttpPorts = configuration.GetSection("WebHttpPorts").Get<IEnumerable<string>>()?.ToArray();
            string[] webHttpsPorts = configuration.GetSection("WebHttpsPorts").Get<IEnumerable<string>>()?.ToArray();
            string CertificateFileName = configuration.GetSection("CertificateFileName").Value;
            string CertificatePassword = configuration.GetSection("CertificatePassword").Value;

            List<string> listHost = new List<string>();
            for (int i = 0; i < webHttpPorts?.Length; i++)//Http
            {
                if (webHttpPorts[i].Trim() == "")
                {
                    continue;
                }
                listHost.Add($@"http://*:{webHttpPorts[i].Trim()}");
            }
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddLog4Net();//需要配置文件
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseUrls(string.Join(";", listHost))
                    .ConfigureKestrel((context, options) =>
                    {
                        //所有controller都不限制post的body大小
                        options.Limits.MinResponseDataRate = null;
                        options.Limits.MaxRequestBodySize = long.MaxValue;
                        options.Limits.MaxConcurrentConnections = long.MaxValue;
                        options.Limits.MaxResponseBufferSize = null;
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
                    }).UseStartup<Startup>();
                });
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            KillWebSocket();
        }
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            KillWebSocket();
        }
        private static void KillWebSocket()
        {
            List<WebSocketInfo> userSocket = SocketHandler._userSocket;
            for (int i = userSocket.Count() - 1; i >= 0; i--)
            {
                if (userSocket.Count > (i + 1) && userSocket[i].webSocket.State == WebSocketState.Open)
                {
                    userSocket[i].webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None);
                }
            }
        }
    }
}
