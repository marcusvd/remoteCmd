using System.Diagnostics;
using remoteCmd.Tasks.Scripts;
namespace remoteCmd.Tasks.Reports;
public static class WindowsLogsManagement
{
    public async static Task<string> ExportEventLogsToEvtx(string logName, string filePath, AppSettings _appsettings, string? startDate = "", string? endDate = "")
    {
        string command = "cmd.exe";
        string arguments = $"/c wevtutil epl {logName} \"{filePath}\"";
        
        if(File.Exists(filePath))
            File.Delete(filePath);

        await Task.Run(() => Tools.ProcessExecutorNoWaitCmdLine(command, arguments, _appsettings)
        );
        return filePath;
    }

    public async static Task<string> ExportEventLogsIntervalDate(string logName, string filePath, AppSettings _appsettings, string? startDate = "", string? endDate = "")
    {
        string command = "cmd.exe";
        string arguments = $"/c wevtutil qe {logName} /q:\"*[System[TimeCreated[@SystemTime>='{startDate}T00:00:00.000Z' and @SystemTime<='{endDate}T23:59:59.999Z']]]\" /f:text > \"{filePath}\"";
        await Task.Run(() => Tools.ProcessExecutorNoWaitCmdLine(command, arguments, _appsettings)
        );

        return filePath;
    }

}