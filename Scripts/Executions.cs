public class Executions
{

    public static void ExecutionsToExecute(string body, Appsettings _appSettingsJson)
    {
        Console.WriteLine($"Body: {body}");
        var shutdown = body.Contains("shutdown");
        if (shutdown)
            Basics.Shutdown();

        var logoff = body.Contains("logoff");
        if (logoff)
            Basics.Logoff();

        var reboot = body.Contains("reboot");
        if (reboot)
            Basics.Reboot();

        var hardware = body.Contains("hardware");
        if (hardware)
            Basics.GetHardwareReport(_appSettingsJson);

        // Specify the event log to export (example: "Application", "System", "Security")
        // Specify the path and file name where the logs will be save
        var eventLogsApplication = body.Contains("EventLogs(Application)");

        if (eventLogsApplication)
            Basics.GetWindowsLogs("Application", $"attachments\\WindowsLogs\\Application.evtx", _appSettingsJson);

        var eventLogsSystem = body.Contains("EventLogs(System)");
        if (eventLogsSystem)
            Basics.GetWindowsLogs("System", $"attachments\\WindowsLogs\\System.evtx", _appSettingsJson);

        var eventLogsSecurity = body.Contains("EventLogs(Security)");
        if (eventLogsSecurity)
            Basics.GetWindowsLogs("Security", $"attachments\\WindowsLogs\\Security.evtx", _appSettingsJson);
    }
}