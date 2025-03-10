using remoteCmd.Tasks.Scripts;
using remoteCmd.Tasks.Useful;

namespace remoteCmd.Tasks.Basics;
public class GetScreen
{
    public static void GetPrintScreen(AppSettings _appSettings)
    {
        string pathSystem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\PrintScreen");
        string fileName = "screenshot_all_monitors.png";
        // string user = Environment.UserName;

        string taskName = "RemoteCmd-GetPrintScreen";
        string command = "schtasks.exe";
        string param = $"/create /tn {taskName} /tr \"powershell.exe -NoProfile -ExecutionPolicy Bypass -File '{Path.Combine(pathSystem, "getPrint.ps1")} -savePath {Path.Combine(pathSystem, fileName)}'\" /sc onlogon /IT /f";
       Advanced.ScheduledTasksActions(taskName, "delete", _appSettings);
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, $"Scheduled task created. -> Get Screen", false);
        Thread.Sleep(5000);
        Advanced.ScheduledTasksActions(taskName, "run", _appSettings);
       // Advanced.ScheduledTasksActions(taskName, "delete", _appSettings);


        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Result error returned - {Environment.MachineName} - {DateTime.Now}", "", Path.Combine(pathSystem, fileName), _appSettings);

    }
}