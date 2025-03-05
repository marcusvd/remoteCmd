using System.Diagnostics;
using System.Management.Automation;

public class Tools
{
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

            ProcessExecutor(processStartInfo, noEmailReturnTasks, command, _appSettings, cmdOutput);

        }
        catch (Exception ex)
        {
            ReturnsExeciutions.ReturnsEmails(ex.Message, command, "Result error returned", _appSettings);
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
    public static void PowershellAddScript(string script)
    {
        using (PowerShell ps = PowerShell.Create())
        {
            ps.AddScript(script);
            ps.Invoke();
        }
    }







}