namespace remoteCmd.Tasks.Reports;

public static class HardwareCalledTasks
{
    public async static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
    {
        var hardware = body.Contains("hardware", StringComparison.OrdinalIgnoreCase);
        if (hardware)
            Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Hardware Report - {Environment.MachineName} - {DateTime.Now}", await HardwareManagement.GetHardwareReportAsync(), "", _appSettings);

    }



}
