namespace remoteCmd.Tasks.Reports;

public static class WindowsLogsCalledTasks
{
    // public async static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
    // {

    //     //Log with attachments file
    //     var pathSave = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs");


    //     var eventLogsApplication = body.Contains("EventLogs(Application)", StringComparison.OrdinalIgnoreCase);
    //     if (eventLogsApplication)
    //         Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"EventLogs Windows Application - {Environment.MachineName} - {DateTime.Now}", $"EventLogs Windows Application, folowing attachmented.", await WindowsLogsManagement.ExportEventLogsToEvtx("Application", $"{pathSave}\\Application.evtx", _appSettings), _appSettings);
    //     //BasicsManagement.GetWindowsLogs("Application", $"{pathSave}\\Application.evtx", _appSettings);

    //     var eventLogsSystem = body.Contains("EventLogs(System)", StringComparison.OrdinalIgnoreCase);
    //     if (eventLogsSystem)
    //         BasicsManagement.GetWindowsLogs("System", $"{pathSave}\\System.evtx", _appSettings);

    //     var eventLogsSecurity = body.Contains("EventLogs(Security)", StringComparison.OrdinalIgnoreCase);
    //     if (eventLogsSecurity)
    //         BasicsManagement.GetWindowsLogs("Security", $"{pathSave}\\Security.evtx", _appSettings);

    // }


    public async static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
    {

        bool logsAttached =
        body.Contains("EventLogs(Application)", StringComparison.OrdinalIgnoreCase)
        ||
        body.Contains("EventLogs(System)", StringComparison.OrdinalIgnoreCase)
        ||
        body.Contains("EventLogs(Security)", StringComparison.OrdinalIgnoreCase);

        if (logsAttached)
            EventLogsAttachments(body, _appSettings);
    }


    private async static void EventLogsAttachments(string body, AppSettings _appSettings)
    {
        string pathSystem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs");
        string logName = string.Empty;
        string fileExtension = ".evtx";
        string pathFileNameToSave = string.Empty;

        var eventLogsApplication = body.Contains("EventLogs(Application)", StringComparison.OrdinalIgnoreCase);
        if (eventLogsApplication)
        {
            logName = "Application";
            pathFileNameToSave = Path.Combine(pathSystem, logName + fileExtension);
        }

        var eventLogsSystem = body.Contains("EventLogs(System)", StringComparison.OrdinalIgnoreCase);
        if (eventLogsSystem)
        {
            logName = "System";
            pathFileNameToSave = Path.Combine(pathSystem, logName + fileExtension);
        }

        var eventLogsSecurity = body.Contains("EventLogs(Security)", StringComparison.OrdinalIgnoreCase);
        if (eventLogsSecurity)
        {
            logName = "Security";
            pathFileNameToSave = Path.Combine(pathSystem, logName + fileExtension);
        }

        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"EventLogs Windows {logName} - {Environment.MachineName} - {DateTime.Now}", $"EventLogs Windows {logName}, folowing attachmented.", await WindowsLogsManagement.ExportEventLogsToEvtx("Application", pathFileNameToSave, _appSettings), _appSettings);

    }

}
