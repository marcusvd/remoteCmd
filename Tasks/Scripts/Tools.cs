using System.Diagnostics;
using System.Management.Automation;
namespace remoteCmd.Tasks.Scripts;
public class Tools
{
    public static void ProcessExecutorNoWaitCmdLine(string command, string param, AppSettings _appSettings, string? cmdOutput = "", bool noEmailReturnTasks = true)
    {

        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = command,
                Arguments = param,
                Verb = "runas", // run as administrator
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            ProcessExecutor(processStartInfo, noEmailReturnTasks, command, _appSettings, cmdOutput);

        }
        catch (Exception ex)
        {
            Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Result error returned - {Environment.MachineName} - {DateTime.Now}", $"{ex.Message}", "", _appSettings);
            Console.WriteLine($"Error when execute command: {ex.Message}");
            EventLog.WriteEntry("RemoteCmd", ex.Message, EventLogEntryType.Error);
            EventLog.WriteEntry("RemoteCmd", $"ProcessExecutorCmdLine {command} {param}", EventLogEntryType.Error);
        }
    }
    private static void ProcessExecutor(ProcessStartInfo processStartInfo, bool noEmailReturnTasks, string command, AppSettings _appSettings, string? cmdOutput = "")
    {
        using (Process process = new Process())
        {
            process.StartInfo = processStartInfo;
            process.Start();
            ProcessExecutorNotification(process, noEmailReturnTasks, command, _appSettings, cmdOutput);
        }
    }
    private static void ProcessExecutorNotification(Process process, bool noEmailReturnTasks, string command, AppSettings _appSettings, string? cmdOutput = "")
    {
        if (noEmailReturnTasks)
        {
            bool cmdOutputChek = string.IsNullOrEmpty(cmdOutput);
            string cmdOutputNewValue = string.Empty;
            // if (!cmdOutputChek)
            //     cmdOutputNewValue = cmdOutput.Split('-')[0];

            string StandardOutput = process.StandardOutput.ReadToEnd();
            string StandardError = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(StandardOutput))
                Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"{(!cmdOutputChek ? cmdOutput : command)} - Result action Output - {Environment.MachineName} - {DateTime.Now}", $"{StandardOutput}", "", _appSettings);


            if (!string.IsNullOrEmpty(StandardError))
                Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"{(!cmdOutputChek ? cmdOutput : command)} - Result error action returned - {Environment.MachineName} - {DateTime.Now}", $"{StandardError}", "", _appSettings);
           
            if (string.IsNullOrEmpty(StandardOutput) && string.IsNullOrEmpty(StandardError))
                Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"{(!cmdOutputChek ? cmdOutput : command)} - the command was received. - {Environment.MachineName} - {DateTime.Now}", cmdOutput ?? "", "", _appSettings);

            Console.WriteLine(StandardOutput);
            Console.WriteLine(StandardError);

            if (!string.IsNullOrEmpty(StandardError))
                EventLog.WriteEntry("RemoteCmd", $"ProcessExecutorCmdLine {command} Error: {StandardError}", EventLogEntryType.Error);

            if (!string.IsNullOrEmpty(StandardOutput))
                EventLog.WriteEntry("RemoteCmd", $"ProcessExecutorCmdLine {command} Info: {StandardOutput}", EventLogEntryType.Information);

        }
    }


    public static void ProcessExecutorWaitCmdLine(string command, string param, AppSettings _appSettings, string? cmdOutput = "", bool noEmailReturnTasks = true)
    {

        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = param,
                Verb = "runas", // run as administrator
                CreateNoWindow = true,
                UseShellExecute = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            ProcessExecutorWait(processStartInfo, noEmailReturnTasks, command, _appSettings, cmdOutput);

        }
        catch (Exception ex)
        {
            // ReturnsExeciutions.ReturnsEmails(ex.Message, command, "Result error returned", _appSettings);
            Console.WriteLine($"Error when execute command: {ex.Message}");
            EventLog.WriteEntry("RemoteCmd", ex.Message, EventLogEntryType.Error);
            EventLog.WriteEntry("RemoteCmd", $"ProcessExecutorCmdLine {command} {param}", EventLogEntryType.Error);
        }
    }
    private static void ProcessExecutorWait(ProcessStartInfo processStartInfo, bool noEmailReturnTasks, string command, AppSettings _appSettings, string? cmdOutput = "")
    {
        try
        {
            using (Process process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    ProcessExecutorNotificationWait("TEST2", noEmailReturnTasks, command, _appSettings, cmdOutput);

                }
                else
                    ProcessExecutorNotificationWait("TEST1", noEmailReturnTasks, command, _appSettings, cmdOutput);
            }
        }
        catch (Exception ex)
        {
            ProcessExecutorNotificationWait("TEST", noEmailReturnTasks, command, _appSettings, ex.ToString());
        }
    }
    private static void ProcessExecutorNotificationWait(string processResult, bool noEmailReturnTasks, string command, AppSettings _appSettings, string? cmdOutput = "")
    {
        if (noEmailReturnTasks)
        {
            string StandardOutput = processResult;
            string StandardError = processResult;

            if (!string.IsNullOrEmpty(StandardOutput))
                // ReturnsExeciutions.ReturnsEmails(command, "Result action Output", StandardOutput, _appSettings);

            if (!string.IsNullOrEmpty(StandardError))
                // ReturnsExeciutions.ReturnsEmails(command, "Result error action returned", StandardError, _appSettings);

            if (string.IsNullOrEmpty(StandardOutput) && string.IsNullOrEmpty(StandardError))
                // ReturnsExeciutions.ReturnsEmails(command, "the command was received.", cmdOutput ?? "", _appSettings);

            Console.WriteLine(StandardOutput);
            Console.WriteLine(StandardError);

            if (!string.IsNullOrEmpty(StandardError))
                EventLog.WriteEntry("RemoteCmd", $"ProcessExecutorCmdLine {command} Error: {StandardError}", EventLogEntryType.Error);

            if (!string.IsNullOrEmpty(StandardOutput))
                EventLog.WriteEntry("RemoteCmd", $"ProcessExecutorCmdLine {command} Info: {StandardOutput}", EventLogEntryType.Information);

        }
    }
}