using System;
using System.Diagnostics;
using Microsoft.Win32;

public class Basics
{

    public static void Shutdown(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/s /t 15";
        Tools.ProcessExecutorCmdLine(command, param, _appSettings, "Shutdown in 15 seconds.");
    }
    public static void Logoff(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/l";
        Tools.ProcessExecutorCmdLine(command, param, _appSettings, "Logoff action executed.");
    }
    public static void Reboot(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/r /t 15";
        Tools.ProcessExecutorCmdLine(command, param, _appSettings, "Reboot in 15 seconds.");
    }
    public static void GetIpAll(AppSettings _appSettings)
    {
        string command = "powershell";
        string param = "Get-NetIPConfiguration -All";
        Tools.ProcessExecutorCmdLine(command, param, _appSettings);
    }

    public async static void GetListAllInstalledSoftware(AppSettings _appSettings)
    {
        string query = "Win32_Product";

        var values = new KeyValuePair<string, string>[]{
            new KeyValuePair<string, string>("Name", "Name"),
            new KeyValuePair<string, string>("Version", "Version"),
            new KeyValuePair<string, string>("Vendor", "Vendor"),
        };

        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Software Report - {Environment.MachineName} - {DateTime.Now}", await SoftwareReport.GetGeneralSystemInformation("INSTALLED PROGRAMS LIST:", values, query), "", _appSettings);

    }

    public async static void GetHardwareReport(AppSettings _appSettings)
    {
        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Hardware Report - {Environment.MachineName} - {DateTime.Now}", await HardwareReport.GetHardwareReportAsync(), "", _appSettings);
    }
    public static async void GetWindowsLogs(string logName, string filePath, AppSettings _appSettings)
    {
        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"EventLogs Windows {logName} - {Environment.MachineName} - {DateTime.Now}", $"EventLogs Windows {logName}, folowing attachmented.", await WindowsLogs.ExportEventLogsToEvtx(logName, filePath, _appSettings), _appSettings);
    }
    public static async void GetWindowsLogsIntervalDate(string logName, string filePath, AppSettings _appSettings, string? startDate = "", string? endDate = "")
    {
        Console.WriteLine(File.ReadAllText(await WindowsLogs.ExportEventLogsIntervalDate(logName, filePath, _appSettings, startDate, endDate)));
      //  Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"EventLogs Windows {logName} - {Environment.MachineName} - {DateTime.Now}", File.ReadAllText(await WindowsLogs.ExportEventLogsIntervalDate(logName, filePath, _appSettings, startDate, endDate)), "", _appSettings);
    }

    public static void PowershellScriptRun(string scriptPath, AppSettings _appSettings)
    {
        string command = $"powershell";
        string param = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}";
        Tools.ProcessExecutorCmdLine(command, param, _appSettings, $"Executed:-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}");
    }


}