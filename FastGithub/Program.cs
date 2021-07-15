﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FastGithub
{
    class Program
    {
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().RunWithWindowsServiceControl();
        }

        /// <summary>
        /// 创建host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseBinaryPathContentRoot()
                .UseDefaultServiceProvider(c =>
                {
                    c.ValidateOnBuild = false;
                })
                .ConfigureAppConfiguration(c =>
                {
                    c.AddJsonFile("appsettings.github.json", optional: true);
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddAppUpgrade();
                    services.AddGithubDns(ctx.Configuration);
                    services.AddGithubReverseProxy(ctx.Configuration);
                    services.AddGithubScanner(ctx.Configuration);
                })
                .ConfigureWebHostDefaults(web =>
                {
                    web.Configure(app => app.UseGithubReverseProxy());
                    web.UseKestrel(kestrel => kestrel.ListenGithubReverseProxy("FastGithub.cer", "FastGithub.key"));
                });
        }
    }
}
