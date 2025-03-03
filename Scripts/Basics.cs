using System;
using System.Diagnostics;
using Microsoft.Win32;

public class Basics
{

    public static void Shutdown(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/s /t 15";
        Basics.ProcessExecutorCmdLine(command, param, _appSettings, "Shutdown in 15 seconds.");
    }
    public static void Logoff(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/l";
        Basics.ProcessExecutorCmdLine(command, param, _appSettings, "Logoff action executed.");
    }
    public static void Reboot(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/r /t 15";
        Basics.ProcessExecutorCmdLine(command, param, _appSettings, "Reboot in 15 seconds.");
    }
    public static void GetIpAll(AppSettings _appSettings)
    {
        string command = "powershell";
        string param = "Get-NetIPConfiguration -All";
        Basics.ProcessExecutorCmdLine(command, param, _appSettings);
    }
    public async static void GetHardwareReport(AppSettings _appSettings)
    {
        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Hardware Report - {Environment.MachineName} - {DateTime.Now}", await HardwareReport.GetHardwareReportAsync(), "", _appSettings);
    }
    public static async void GetWindowsLogs(string logName, string filePath, AppSettings _appSettings)
    {
        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"EventLogs Windows {logName} - {Environment.MachineName} - {DateTime.Now}", $"EventLogs Windows {logName}, folowing attachmented.", await WindowsLogs.ExportEventLogsToEvtx(logName, filePath), _appSettings);
    }

    public static void PowershellScriptRun(string scriptPath, AppSettings _appSettings)
    {
        string command = $"powershell";
        string param = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}";
        Basics.ProcessExecutorCmdLine(command, param, _appSettings, $"Executed:-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}");
    }
    public static void ProcessExecutorCmdLine(string command, string param, AppSettings _appSettings, string? cmdOutput = "", bool noEmailReturnTasks = true)
    {


        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = command,
                Arguments = param,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                if (noEmailReturnTasks)
                {
                    string StandardOutput = process.StandardOutput.ReadToEnd();
                    string StandardError = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(StandardOutput))
                        ReturnsExeciutions.ReturnsEmails(command, "Result action Output", StandardOutput, _appSettings);

                    if (!string.IsNullOrEmpty(StandardError))
                        ReturnsExeciutions.ReturnsEmails(command, "Result error action returned", StandardError, _appSettings);
                        
                    if (string.IsNullOrEmpty(StandardOutput) && string.IsNullOrEmpty(StandardError))
                        ReturnsExeciutions.ReturnsEmails(command, "the command was received.", cmdOutput ?? "", _appSettings);

                    Console.WriteLine(StandardOutput);
                    Console.WriteLine(StandardError);

                    if (!string.IsNullOrEmpty(StandardError))
                        EventLog.WriteEntry("RemoteCmd", $"ProcessExecutorCmdLine {command} Error: {StandardError}", EventLogEntryType.Error);

                    if (!string.IsNullOrEmpty(StandardOutput))
                        EventLog.WriteEntry("RemoteCmd", $"ProcessExecutorCmdLine {command} Info: {StandardOutput}", EventLogEntryType.Information);

                }
            }

        }
        catch (Exception ex)
        {
            ReturnsExeciutions.ReturnsEmails(ex.Message, command, "Result error returned", _appSettings);
            Console.WriteLine($"Error when execute command: {ex.Message}");
            EventLog.WriteEntry("RemoteCmd", ex.Message, EventLogEntryType.Error);
            EventLog.WriteEntry("RemoteCmd", $"ProcessExecutorCmdLine {command} {param}", EventLogEntryType.Error);
        }
    }


}