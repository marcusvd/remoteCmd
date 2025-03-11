using Microsoft.Win32;
using remoteCmd.Tasks.Basic;
using remoteCmd.Tasks.RegistryOperations;
using remoteCmd.Tasks.Useful;
namespace remoteCmd.Tasks.Scripts;
public static class Advanced
{
    private static void ExecuteScriptElevatedAfterLogon(string filePath)
    {
        TextFile.ScriptModify(filePath, TextFile.DisableAutoLogon);

        RegistryManagement.CreateRegistryEntry(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", "RemoteCmd Elevated Action", $"powershell.exe -File \"{filePath}\"");
    }
    public static void LogResultActionReturnEmail(string filePath)
    {
        TextFile.ScriptModify(filePath, TextFile.LogResultActionReturnEmail);
    }
    private static void GetSaveCurrentUserNameLogged()
    {
        RegistryManagement.CreateRegistryEntry(Registry.LocalMachine, "SOFTWARE\\RemoteCmd", "current logged user", Environment.UserName);
    }
    private static void ConfigureAutoLogon(string scriptPath, AppSettings _appSettings, string domain, string userName, string password)
    {
        string command = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools\\Autologon.exe");

        string param = $"/accepteula {userName} {domain} {password}";

        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, $"Configure AutoLogon {scriptPath}");
    }
    private static void ScheduledTasksActions(string taskName, string action, AppSettings _appSettings, bool noEmailReturnTasks = false)
    {

        string command = "schtasks.exe";
        string param = string.Empty;

        if (action == "run")
            param = $"/run /tn {taskName}";

        if (action == "delete")
            param = $"/delete /tn {taskName} /f";

        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, $"Scheduled task Executed. -> {taskName}", noEmailReturnTasks);
    }
    public static void DisableAutoLogon()
    {
        RegistryManagement.CreateRegistryEntry(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "AutoAdminLogon", "0");
    }
    public static void AdvancedElevatedExecution(string userName, string domain, string password, string scriptPath, AppSettings _appSettings)
    {
        GetSaveCurrentUserNameLogged();
        ExecuteScriptElevatedAfterLogon(scriptPath);
        LogResultActionReturnEmail(scriptPath);
        ConfigureAutoLogon(scriptPath, _appSettings, userName, domain, password);
        BasicsManagement.Reboot(_appSettings);
    }
    public static void ScheduleBasicTaskPowerShellScript(string scriptPath, AppSettings _appSettings)
    {
        LogResultActionReturnEmail(scriptPath);
        string user = Environment.UserName;

        string taskName = "RemoteCmd-ScheduleBasicTaskPowerShellScript";
        string command = "schtasks.exe";
        string param = $"/create /tn {taskName} /tr \"powershell.exe -NoProfile -ExecutionPolicy Bypass -File '{scriptPath}'\" /sc onlogon /ru {user}  /IT /f";

        ScheduledTasksActions(taskName, "delete", _appSettings);
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, $"Scheduled task created. -> {scriptPath}", false);
        Thread.Sleep(5000);
        ScheduledTasksActions(taskName, "run", _appSettings);
        ScheduledTasksActions(taskName, "delete", _appSettings);
    }
    public static void ScheduleGetScreenShotBasicTask(AppSettings _appSettings)
    {

        string getPrint =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools\\getScreen.exe");

        string taskName = "RemoteCmd-ScheduleGetScreenShotBasicTask";
        string command = "schtasks.exe";
        string param = $"/create /tn {taskName} /tr \"{getPrint}\" /sc onlogon /IT /f";

        ScheduledTasksActions(taskName, "delete", _appSettings);
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, $"Scheduled task created. -> ", false);
        Thread.Sleep(5000);
        ScheduledTasksActions(taskName, "run", _appSettings);
        //ScheduledTasksActions(taskName, "delete", _appSettings);
    }
    public static void ScheduleTaskHighestPowerShellScript(string scriptPath, AppSettings _appSettings, string userName, string password)
    {
        LogResultActionReturnEmail(scriptPath);
        string taskName = "RemoteCmdScheduleTaskHighest";
        string command = "schtasks.exe";
        string param = $"/create /tn {taskName} /tr  \"powershell.exe -NoProfile -ExecutionPolicy Bypass -File '{scriptPath}'\" /sc onlogon /ru {userName} /rp {password} /RL HIGHEST /IT /f";

        ScheduledTasksActions(taskName, "delete", _appSettings, false);
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, $"Scheduled task created. -> {scriptPath}", false);
        Thread.Sleep(5000);
        ScheduledTasksActions(taskName, "run", _appSettings);
        ScheduledTasksActions(taskName, "delete", _appSettings, false);
    }
    public static void PowershellScriptRun(string scriptPath, AppSettings _appSettings)
    {
        string command = $"powershell";
        string param = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}";
        string scriptFile = Path.GetFileName(scriptPath);

        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, $"Executed:   {scriptFile.ToUpper()} -> {Environment.MachineName} - {DateTime.Now}");
    }

    //"C:\Program Files (x86)\NoStopTi\tools\psexec.exe" -u localhost\administrador -p http2025$ powershell.exe -NoProfile -Command Start-Process "'C:\\Program Files (x86)\\NoStopTi\\tools\\Autologon.exe' administrador, localhost, http2025$"
    // public static void test(string scriptPath, AppSettings _appSettings, string domain, string userName, string password)
    // {
    //     string command = $"\"{AppDomain.CurrentDomain.BaseDirectory}\\tools\\psexec.exe\"";
    //    // string autoLogonExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools\\Autologon.exe");
    //     string param = $" -accepteula -u localhost\\administrador -p http2025$ powershell.exe -File \"{scriptPath}";
    // //     string command = $"\"{AppDomain.CurrentDomain.BaseDirectory}\\tools\\psexec.exe\" -u localhost\\administrador -p http2025$";
    // //    // string autoLogonExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools\\Autologon.exe");
    // //     string param = $"powershell.exe -File \"{scriptPath}";
    //     Console.WriteLine("psexec" + command);
    //     Console.WriteLine($"{param}\"");

    //     Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, $"Scheduled task created. -> {scriptPath}");
    //     //get current user logged and save in a key on registry.
    //    // RegistryManagement.CreateRegistryEntry(Registry.LocalMachine, "SOFTWARE\\RemoteCmd", "current logged user", Environment.UserName);
    // }

}