using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>

        Host.CreateDefaultBuilder(args)

        .UseWindowsService() //Configure the app to be executed as a Windows Service

        .ConfigureAppConfiguration((hostContext, config) =>
        {
            if (string.IsNullOrEmpty(JsonManagement.jsonPath)) JsonManagement.jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSettings.json");
            //load settings from appsettings.json
            config.AddJsonFile(JsonManagement.jsonPath, optional: false, reloadOnChange: true);

        })

        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Connect>();
            services.Configure<HostOptions>(options =>
            {
                options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });
        });

    // public static string GetJsonAppsettingsPath(RegistryKey root, string key, string name, string value)
    // {
    //     var isExistResult = RegistryGetSet.isKeyNameValueExists(root, key, name);

    //     if (!isExistResult)
    //         RegistryGetSet.CreateRegistryEntry(root, key, name, value);
    //     else
    //     {
    //         if (!string.IsNullOrEmpty(RegistryGetSet.GetRegistryValue(root, key, name)))
    //             return RegistryGetSet.GetRegistryValue(root, key, name);
    //         else
    //             return Path.Combine(BasePath.Path, value);

    //     }

    //     return Path.Combine(BasePath.Path, value);
    //}


}