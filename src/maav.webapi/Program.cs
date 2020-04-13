using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MAAV.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostBuilder, config) => 
                {
                    var enviroment = hostBuilder.HostingEnvironment;
                    config
                        .SetBasePath(enviroment.ContentRootPath)
                        .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("connectionStrings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appSettings.{enviroment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables("MAAV_")
                        .AddCommandLine(args);
                })
                .UseDefaultServiceProvider(options => {
                    options.ValidateScopes = false;
                });
    }
}
