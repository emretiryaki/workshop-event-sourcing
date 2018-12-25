using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using static System.Environment;

namespace Reviews.Service.QueryApi
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            await Console.Error.WriteLineAsync($"Query Service starting ({GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"})...");

            try
            {
                var configuration = BuildConfiguration(args);
                await Console.Error.WriteLineAsync("Configuration built successfully.");
                await CreateWebHostBuilder(configuration,args).Build().RunAsync();

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(IConfiguration configuration,string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .ConfigureServices(services => services.AddSingleton(configuration))
                .UseUrls("http://127.0.0.1:5005")
                .UseStartup<Startup>();
        
        private static IConfiguration BuildConfiguration(string[] args)
            => new ConfigurationBuilder()
                .SetBasePath(CurrentDirectory)
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"appsettings.{GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
    }
}
