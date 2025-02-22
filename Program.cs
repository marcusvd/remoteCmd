using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Configuration;
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).UseWindowsService() //Configure the app to be executed as a Windows Service



        .ConfigureAppConfiguration((hostContext, config) =>
        {
            //load settings from appsettings.json
            config.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "path.json"), optional: false, reloadOnChange: true);
        })

        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Connect>();
        });

}