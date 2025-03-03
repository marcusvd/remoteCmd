
public class Executions
{
    public static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
    {
        Console.WriteLine($"Body: {body}");

        var shutdown = body.Contains("shutdown");
        if (shutdown)
            Basics.Shutdown(_appSettings);

        var logoff = body.Contains("logoff");
        if (logoff)
            Basics.Logoff(_appSettings);

        var reboot = body.Contains("reboot");
        if (reboot)
            Basics.Reboot(_appSettings);
       
        var getIpAll = body.Contains("getIpAll");
        if (getIpAll)
            Basics.GetIpAll(_appSettings);

        var hardware = body.Contains("hardware");
        if (hardware)
            Basics.GetHardwareReport(_appSettings);

        var eventLogsApplication = body.Contains("EventLogs(Application)");
        if (eventLogsApplication)
            Basics.GetWindowsLogs("Application", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs\\Application.evtx"), _appSettings);

        var eventLogsSystem = body.Contains("EventLogs(System)");
        if (eventLogsSystem)
            Basics.GetWindowsLogs("System", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs\\System.evtx"), _appSettings);

        var eventLogsSecurity = body.Contains("EventLogs(Security)");
        if (eventLogsSecurity)
            Basics.GetWindowsLogs("Security", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs\\Security.evtx"), _appSettings);
    }
}