using System;
using System.Diagnostics;

public class Basics
{
   
    public static void Shutdown()
    {
        string command = "shutdown";
        string param = "/s /t 0";
        Basics.ProcessExecutorCmdLine(command, param);
    }
    public static void Logoff()
    {
        string command = "shutdown";
        string param = "/l";
        Basics.ProcessExecutorCmdLine(command, param);
    }
    public static void Reboot()
    {
        string command = "shutdown";
        string param = "/r /t 0";
        Basics.ProcessExecutorCmdLine(command, param);
    }
    public async static void GetHardwareReport(Appsettings _appSettingsJson)
    {


        EmailSender.SendEmail(_appSettingsJson.ServerSmtp.UserName, $"Hardware Report - {Environment.MachineName} - {DateTime.Now}", await HardwareReport.GetHardwareReportAsync(), "", _appSettingsJson);
    }
    public static async void GetWindowsLogs(string logName, string filePath,Appsettings _appSettingsJson)
    {

        EmailSender.SendEmail(_appSettingsJson.ServerSmtp.UserName, $"EventLogs Windows {logName} - {Environment.MachineName} - {DateTime.Now}", $"EventLogs Windows {logName}, folowing attachmented.", await WindowsLogs.ExportEventLogsToEvtx(logName, filePath), _appSettingsJson);

    }



    public static void ProcessExecutorCmdLine(string command, string param)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo(command, param);
        processStartInfo.CreateNoWindow = true;
        processStartInfo.UseShellExecute = false;

        try
        {
            // Inicia o processo
            Process.Start(processStartInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error when execute command: {ex.Message}");
        }
    }

    public static void ExecutePowerShellScript(string scriptPath)
    {
        try
        {

            // Create a new process to run PowerShell
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            //Start the process and capture the output
            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();

                TextFileWriter.Write($"attachments\\ScriptsToExecute\\Execution log from ${scriptPath.Split("\\").Last()}", $"{output}");


                if (!string.IsNullOrEmpty(errors))
                {
                    TextFileWriter.Write($"attachments\\ScriptsToExecute\\Error log from ${scriptPath.Split("\\").Last()}", $"{errors}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao executar script PowerShell: {ex.Message}");
        }
    }
}