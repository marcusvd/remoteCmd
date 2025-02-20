using System.Diagnostics;

public static class WindowsLogs
{
    public async static Task<string> ExportEventLogsToEvtx(string logName, string filePath)
    {
        try
        {
            // Build the command to export the logs using wevtutil
            string command = $"wevtutil epl {logName} \"{filePath}\"";

            // Executar o comando
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {command}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            // Capture the command output
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Display a success message
            Console.WriteLine($"Logs successfully exported to file: {filePath}");
            return await Task.FromResult<string>(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting logs: {ex.Message}");
            return string.Empty;
        }
    }
    // string logName = "Application"; // Specify the event log to export (example: "Application", "System", "Security")
    //     string filePath = "event_log_export.evtx"; // Specify the path and file name where the logs will be saved

    //     ExportEventLogsToEvtx(logName, filePath);
}