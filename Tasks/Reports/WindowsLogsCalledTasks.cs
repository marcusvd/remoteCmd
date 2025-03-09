using Org.BouncyCastle.Asn1.Cms;

namespace remoteCmd.Tasks.Reports;

public static class WindowsLogsCalledTasks
{
    static string pathSystem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs");
    static string logName = string.Empty;
    static string pathFileNameToSave = string.Empty;

    public async static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
    {

        bool logsAttached = body.Contains("GetEventLogs", StringComparison.OrdinalIgnoreCase);
        if (logsAttached)
            EventLogsAttachments(body, _appSettings);

        var getListEventLogs = body.Contains("GetListEventLogs", StringComparison.OrdinalIgnoreCase);
        if (getListEventLogs)
        {
            logName = "logList";
            pathFileNameToSave = Path.Combine(pathSystem, logName + ".txt");
            var resultFile = await WindowsLogsManagement.GetListEventLogs(pathFileNameToSave, _appSettings);
            Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Attached Log options list - {Environment.MachineName} - {DateTime.Now}", $"All lists log options.", resultFile, _appSettings);
        }
    }


    private async static void EventLogsAttachments(string body, AppSettings _appSettings)
    {
        var bodySplit = body.Split('|');

        var getStartWith = bodySplit.FirstOrDefault(x => x.StartsWith("Log:", StringComparison.OrdinalIgnoreCase));

        if (getStartWith != null)
        {
            int startIndex = getStartWith.IndexOf('[') + 1;
            int endIndex = getStartWith.IndexOf(']');
            string logsNames = getStartWith.Substring(startIndex, endIndex - startIndex);
            var logName = logsNames.Split('#');

            foreach (var log in logName)
            {
                var pathFileNameToSave = Path.Combine(pathSystem, $"{log}.evtx");

                var filePath = await WindowsLogsManagement.ExportEventLogsToEvtx(log, pathFileNameToSave, _appSettings);


                while (!File.Exists(filePath) || TimeSpan.FromMinutes(1) < (DateTime.Now - File.GetLastWriteTime(filePath)))
                {
                    Thread.Sleep(1000);
                }

                Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"EventLogs Windows {log} - {Environment.MachineName} - {DateTime.Now}", $"EventLogs Windows {log}, folowing attachmented.", filePath, _appSettings);

            }

        }
    }
}