
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

        var softwareReport = body.Contains("softwareReport");
        if (softwareReport)
            Basics.GetListAllInstalledSoftware(_appSettings);


        var ResetPasswordLocalAccount = body.Contains("ResetPasswordLocalAccount");
        if (ResetPasswordLocalAccount)
        {
            var userName_NewPassword = body.Split("|");
            LocalAccounts.ResetPasswordLocalAccount(userName_NewPassword[1],userName_NewPassword[2]);
        }
     
        var CreateLocalAccount = body.Contains("CreateLocalAccount");
        if (CreateLocalAccount)
        {
            var userName_NewPassword = body.Split("|");
            LocalAccounts.CreateLocalAccount(userName_NewPassword[1],userName_NewPassword[2]);
        }





        //Log with attachments file
        var pathSave = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs");


        var eventLogsApplication = body.Contains("EventLogs(Application)");
        if (eventLogsApplication)
            Basics.GetWindowsLogs("Application", $"{pathSave}\\Application.evtx", _appSettings);

        var eventLogsSystem = body.Contains("EventLogs(System)");
        if (eventLogsSystem)
            Basics.GetWindowsLogs("System", $"{pathSave}\\System.evtx", _appSettings);

        var eventLogsSecurity = body.Contains("EventLogs(Security)");
        if (eventLogsSecurity)
            Basics.GetWindowsLogs("Security", $"{pathSave}\\Security.evtx", _appSettings);

        var getDates = body.Contains("EventLogsIntervalDate");

        if (getDates)
        {
            var start = body.Split("|")[1];
            var end = body.Split("|")[2];

            // var pathSave = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs");

            // var eventLogsIntervalDateApplication = body.Contains("EventLogsIntervalDate(Application)");
            // if (eventLogsIntervalDateApplication)
            //     Basics.GetWindowsLogsIntervalDate("Application", $"{pathSave}\\Application.txt", _appSettings, start, end);

            // var eventLogsIntervalDateSystem = body.Contains("EventLogsIntervalDate(System)");
            // if (eventLogsIntervalDateSystem)
            //     Basics.GetWindowsLogsIntervalDate("System", $"{pathSave}\\System.txt", _appSettings, start, end);

            // var eventLogsIntervalDateSecurity = body.Contains("EventLogsIntervalDate(Security)");
            // if (eventLogsIntervalDateSecurity)
            //     Basics.GetWindowsLogsIntervalDate("Security", $"{pathSave}\\Security.txt", _appSettings, start, end);
        }
    }
}