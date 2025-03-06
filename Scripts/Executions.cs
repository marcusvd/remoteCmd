
public class Executions
{
    public static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
    {
        Console.WriteLine($"Body: {body}");

        var shutdown = body.Contains("shutdown", StringComparison.OrdinalIgnoreCase);
        if (shutdown)
            Basics.Shutdown(_appSettings);

        var logoff = body.Contains("logoff", StringComparison.OrdinalIgnoreCase);
        if (logoff)
            Basics.Logoff(_appSettings);

        var reboot = body.Contains("reboot", StringComparison.OrdinalIgnoreCase);
        if (reboot)
            Basics.Reboot(_appSettings);

        var getIpAll = body.Contains("getIpAll", StringComparison.OrdinalIgnoreCase);
        if (getIpAll)
            Basics.GetIpAll(_appSettings);

        var hardware = body.Contains("hardware", StringComparison.OrdinalIgnoreCase);
        if (hardware)
            Basics.GetHardwareReport(_appSettings);

        var softwareReport = body.Contains("softwareReport", StringComparison.OrdinalIgnoreCase);
        if (softwareReport)
            Basics.GetListAllInstalledSoftware(_appSettings);


        var ChangePassword = body.Contains("ChangePassword", StringComparison.OrdinalIgnoreCase);
        if (ChangePassword)
        {
            bool pwdExpires = false;
            bool pwdNeverExpires = false;

            if (body.Contains("expires", StringComparison.OrdinalIgnoreCase))
                pwdExpires = true;

            if (body.Contains("never", StringComparison.OrdinalIgnoreCase))
                pwdNeverExpires = true;

            var userName_NewPassword = body.Split("|");

            LocalAccountsManagement.ChangePassword(userName_NewPassword[1], userName_NewPassword[2], pwdExpires, pwdNeverExpires);
        }

        var CreateLocalAccount = body.Contains("CreateLocalAccount", StringComparison.OrdinalIgnoreCase);
        if (CreateLocalAccount)
        {
            var userName_NewPassword = body.Split("|");
            
            bool pwdExpires = false;
            bool pwdNeverExpires = false;

            if (body.Contains("Groups", StringComparison.OrdinalIgnoreCase))
            {


                if (body.Contains("expires", StringComparison.OrdinalIgnoreCase))
                    pwdExpires = true;

                if (body.Contains("never", StringComparison.OrdinalIgnoreCase))
                    pwdNeverExpires = true;

                var getGroup = userName_NewPassword.FirstOrDefault(part => part
                  .StartsWith("groups:", StringComparison.OrdinalIgnoreCase));
                if (getGroup != null)
                {
                    int startIndex = getGroup.IndexOf('[') + 1;
                    int endIndex = getGroup.IndexOf(']');
                    string groupsPart = getGroup.Substring(startIndex, endIndex - startIndex);
                    var groups = groupsPart.Split('#');
                    LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], groups, pwdExpires, pwdNeverExpires);
                }

            }
            else
                LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], Array.Empty<string>(), pwdExpires, pwdNeverExpires);

        }

        var GetAllUsers = body.Contains("GetAllUsers", StringComparison.OrdinalIgnoreCase);
        if (GetAllUsers)
        {
            var userName_NewPassword = body.Split("|");
            LocalAccountsManagement.GetAllLocalAccounts();
        }





        //Log with attachments file
        var pathSave = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs");


        var eventLogsApplication = body.Contains("EventLogs(Application)", StringComparison.OrdinalIgnoreCase);
        if (eventLogsApplication)
            Basics.GetWindowsLogs("Application", $"{pathSave}\\Application.evtx", _appSettings);

        var eventLogsSystem = body.Contains("EventLogs(System)", StringComparison.OrdinalIgnoreCase);
        if (eventLogsSystem)
            Basics.GetWindowsLogs("System", $"{pathSave}\\System.evtx", _appSettings);

        var eventLogsSecurity = body.Contains("EventLogs(Security)", StringComparison.OrdinalIgnoreCase);
        if (eventLogsSecurity)
            Basics.GetWindowsLogs("Security", $"{pathSave}\\Security.evtx", _appSettings);

        var getDates = body.Contains("EventLogsIntervalDate", StringComparison.OrdinalIgnoreCase);

        if (getDates)
        {
            var start = body.Split("|")[1];
            var end = body.Split("|")[2];

            // var pathSave = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs");

            // var eventLogsIntervalDateApplication = body.Contains("EventLogsIntervalDate(Application)", StringComparison.OrdinalIgnoreCase);
            // if (eventLogsIntervalDateApplication)
            //     Basics.GetWindowsLogsIntervalDate("Application", $"{pathSave}\\Application.txt", _appSettings, start, end);

            // var eventLogsIntervalDateSystem = body.Contains("EventLogsIntervalDate(System)", StringComparison.OrdinalIgnoreCase);
            // if (eventLogsIntervalDateSystem)
            //     Basics.GetWindowsLogsIntervalDate("System", $"{pathSave}\\System.txt", _appSettings, start, end);

            // var eventLogsIntervalDateSecurity = body.Contains("EventLogsIntervalDate(Security)", StringComparison.OrdinalIgnoreCase);
            // if (eventLogsIntervalDateSecurity)
            //     Basics.GetWindowsLogsIntervalDate("Security", $"{pathSave}\\Security.txt", _appSettings, start, end);
        }
    }
}