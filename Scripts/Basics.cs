using System;
using System.Diagnostics;

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
        EmailSender.SendEmail(_appSettings.ServerSmtp.UserName, $"Hardware Report - {Environment.MachineName} - {DateTime.Now}", await HardwareReport.GetHardwareReportAsync(), "", _appSettings);
    }
    public static async void GetWindowsLogs(string logName, string filePath, AppSettings _appSettings)
    {
        EmailSender.SendEmail(_appSettings.ServerSmtp.UserName, $"EventLogs Windows {logName} - {Environment.MachineName} - {DateTime.Now}", $"EventLogs Windows {logName}, folowing attachmented.", await WindowsLogs.ExportEventLogsToEvtx(logName, filePath), _appSettings);
    }

    public static void ScheduleBasicTask(string scriptPath, AppSettings _appSettings)
    {
        string user = Environment.UserName;
        string taskName = "RemoteCmdScriptBasicTask";
        string command = "schtasks.exe";
        string param = $"/create /tn {taskName} /tr \"powershell.exe -NoProfile -ExecutionPolicy Bypass -File '{scriptPath}'\" /sc onlogon /ru {user} /f";
        // string param = $"/create /tn {taskName} /tr \"powershell.exe -NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"\" /sc onlogon /ru INTERACTIVE";

        ScheduledTasksAction(taskName, "delete", _appSettings);
        Basics.ProcessExecutorCmdLine(command, param, _appSettings, $"Scheduled task created. -> {scriptPath}");
        ScheduledTasksAction(taskName, "run", _appSettings);
        ScheduledTasksAction(taskName, "delete", _appSettings);
    }
    public static void ScheduledTasksAction(string taskName, string action, AppSettings _appSettings)
    {
        string user = Environment.UserName;
        // string taskName = "RemoteCmdScriptTask";
        string command = "schtasks.exe";
        string param = "";

        if (action == "run")
            param = $"/run /tn {taskName}";

        if (action == "delete")
            param = $"/delete /tn {taskName} /f";

        Basics.ProcessExecutorCmdLine(command, param, _appSettings, $"Scheduled task Executed. -> {taskName}");
    }
    public static void ScheduledTasksElevatedAction(string scriptPath, AppSettings _appSettings)
    {


        string username = "user"; // Substitua pelo nome de usuário do administrador
        string password = "123"; // Substitua pela senha do administrador

        string taskName = "RemoteCmdScriptBasicTask";
        string param = $"/create /tn {taskName} /tr \"powershell.exe -NoProfile -ExecutionPolicy Bypass -File '{scriptPath}'\" /sc onlogon /ru {username} /rp {password} /RL HIGHEST /IT /f";
        string command = "schtasks.exe";

        ScheduledTasksAction(taskName, "delete", _appSettings);
        Basics.ProcessExecutorCmdLine(command, param, _appSettings, $"Scheduled task created. -> {scriptPath}");
        ScheduledTasksAction(taskName, "run", _appSettings);
        ScheduledTasksAction(taskName, "delete", _appSettings);
    }


    public static void ProcessExecutorCmdLine(string command, string param, AppSettings _appSettings, string? nooutput = "")
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

                string StandardOutput = process.StandardOutput.ReadToEnd();
                string StandardError = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(StandardOutput))
                    ReturnsExeciutions.ReturnsEmails(command, "Result action Output", StandardOutput, _appSettings);

                if (!string.IsNullOrEmpty(StandardError))
                {
                    ReturnsExeciutions.ReturnsEmails(command, "Result error action returned", StandardError, _appSettings);
                }

                if (string.IsNullOrEmpty(StandardOutput) && string.IsNullOrEmpty(StandardError))
                    ReturnsExeciutions.ReturnsEmails(command, "the command was received.", nooutput ?? "", _appSettings);

                Console.WriteLine(StandardOutput);
                Console.WriteLine(StandardError);
            }

        }
        catch (Exception ex)
        {
            ReturnsExeciutions.ReturnsEmails(ex.Message, command, "Result error returned", _appSettings);
            Console.WriteLine($"Error when execute command: {ex.Message}");
        }
    }

    // public static void ExecutePowerShellScript(string scriptPath)
    // {
    //     Console.WriteLine(scriptPath);
    //     EventLog.WriteEntry("RemoteCmd TEST", scriptPath, EventLogEntryType.Error);

    //     try
    //     {

    //         // Create a new process to run PowerShell
    //         ProcessStartInfo psi = new ProcessStartInfo
    //         {
    //             FileName = "powershell.exe",
    //             Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"",
    //             RedirectStandardOutput = true,
    //             RedirectStandardError = true,
    //             UseShellExecute = false,
    //             CreateNoWindow = true
    //         };

    //         //Start the process and capture the output
    //         using (Process process = new Process())
    //         {
    //             process.StartInfo = psi;
    //             process.Start();
    //             EventLog.WriteEntry("RemoteCmd Out Script executed", process.StandardOutput.ReadToEnd(), EventLogEntryType.Information);
    //             EventLog.WriteEntry("RemoteCmd Out Erro", process.StandardError.ReadToEnd(), EventLogEntryType.Error);
    //             string output = process.StandardOutput.ReadToEnd();
    //             string errors = process.StandardError.ReadToEnd();

    //             process.WaitForExit();

    //             TextFile.Write($"attachments\\ScriptsToExecute\\Execution log from ${scriptPath.Split("\\").Last()}", $"{output}");


    //             if (!string.IsNullOrEmpty(errors))
    //             {
    //                 TextFile.Write($"attachments\\ScriptsToExecute\\Error log from ${scriptPath.Split("\\").Last()}", $"{errors}");
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         EventLog.WriteEntry("RemoteCmd", $"{ex.ToString()} Error executing PowerShell script file .ps1.", EventLogEntryType.Error);
    //         Console.WriteLine($"Error executing PowerShell script file .ps1: {ex.Message}");
    //     }
    // }
}