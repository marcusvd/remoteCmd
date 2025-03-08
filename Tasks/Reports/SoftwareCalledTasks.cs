namespace remoteCmd.Tasks.Reports;

public static class SoftwareCalledTasks
{
    public static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
    {
        var softwareReport = body.Contains("softwareReport", StringComparison.OrdinalIgnoreCase);
        if (softwareReport)
        {
            SoftwareManagement.GetListAllInstalledSoftware(_appSettings);
        }
    }



}
