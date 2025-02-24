using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;

public class Program
{
    public static string jsonPath = RegistryGetSet.GetRegistryValue(Registry.LocalMachine, "SOFTWARE\\RemoteCmd", "AppSettingsJson");
    // public static string jsonPath = GetJsonAppsettingsPath(Registry.LocalMachine, "SOFTWARE\\RemoteCmd", "AppSettingsJson", "C:\\Program Files (x86)\\NoStopTi\\appSettings.json");
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>

        Host.CreateDefaultBuilder(args).UseWindowsService() //Configure the app to be executed as a Windows Service



        .ConfigureAppConfiguration((hostContext, config) =>
        {
            //load settings from appsettings.json
            config.AddJsonFile(jsonPath, optional: false, reloadOnChange: true);
            // config.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "path.json"), optional: false, reloadOnChange: true);
        })

        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Connect>();
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