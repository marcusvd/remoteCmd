using System.Management;
using System.Text;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

public class SoftwareReport
{
    public static async Task<string> GetGeneralSystemInformation(string title, KeyValuePair<string, string>[] values, string className)
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
                         // else
                         // {
                         //      Console.WriteLine($"{value.Key}: Property '{value.Value}' not found");
                         // }
                     }

                     resultEmail += TextFile.OutPut("");

                     resultEmail += TextFile.OutPut(new string('=', 50));
                 }
             }
         });

        return resultEmail;
    }
}
