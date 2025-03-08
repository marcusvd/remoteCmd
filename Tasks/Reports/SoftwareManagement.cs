using System.Diagnostics;
using System.Management;
namespace remoteCmd.Tasks.Reports;
public static class SoftwareManagement
{
    public async static void GetListAllInstalledSoftware(AppSettings _appSettings)
    {
        string query = "Win32_Product";

        var values = new KeyValuePair<string, string>[]{
            new KeyValuePair<string, string>("Name", "Name"),
            new KeyValuePair<string, string>("Version", "Version"),
            new KeyValuePair<string, string>("Vendor", "Vendor"),
        };

        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Software Report - {Environment.MachineName} - {DateTime.Now}", await GetGeneralSystemInformation("INSTALLED PROGRAMS LIST:", values, query), "", _appSettings);

    }

      private static async Task<string> GetGeneralSystemInformation(string title, KeyValuePair<string, string>[] values, string className)
    {

        string resultEmail = TextFile.OutPut($"{title}");;
        await Task.Run(() =>
         {

             using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM {className}"))
             {

                 ManagementObjectCollection results = searcher.Get();

                 foreach (ManagementObject result in results)
                 {
                     foreach (var value in values)
                     {

                         if (result.Properties[value.Value] != null)

                             resultEmail += TextFile.OutPut($"{value.Key}: {result[value.Value]}");
                     }

                     resultEmail += TextFile.OutPut("");

                     resultEmail += TextFile.OutPut(new string('=', 50));
                 }
             }
         });

        return resultEmail;
    }

}