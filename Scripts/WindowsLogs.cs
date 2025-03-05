using System.Diagnostics;

public static class WindowsLogs
{
    public async static Task<string> ExportEventLogsToEvtx(string logName, string filePath, AppSettings _appsettings, string? startDate = "", string? endDate = "")
    {
        string command = "cmd.exe";
        string arguments = $"/c wevtutil epl {logName} \"{filePath}\"";
        await Task.Run(() => Tools.ProcessExecutorCmdLine(command, arguments, _appsettings)
        );
        return filePath;
    }

    public async static Task<string> ExportEventLogsIntervalDate(string logName, string filePath, AppSettings _appsettings, string? startDate = "", string? endDate = "")
    {
        string command = "cmd.exe";
        string arguments = $"/c wevtutil qe {logName} /q:\"*[System[TimeCreated[@SystemTime>='{startDate}T00:00:00.000Z' and @SystemTime<='{endDate}T23:59:59.999Z']]]\" /f:text > \"{filePath}\"";
        await Task.Run(() => Tools.ProcessExecutorCmdLine(command, arguments, _appsettings)
        );

        return filePath;
    }

}