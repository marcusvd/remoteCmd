using Microsoft.Win32;

public static class Advanced
{
    public static void ExecuteScriptElevatedAfterLogon(string filePath)
    {
        TextFile.ScriptModify(filePath, TextFile.DisableAutoLogon);

        RegistryManagement.RegistryOperations.CreateRegistryEntry(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", "RemoteCmd Elevated Action", $"powershell.exe -File \"{filePath}\"");
    }
    public static void GetSaveCurrentUserNameLogged()
    {
        RegistryManagement.RegistryOperations.CreateRegistryEntry(Registry.LocalMachine, "SOFTWARE\\RemoteCmd", "current logged user", Environment.UserName);
    }
    public static void ConfigureAutoLogon(string scriptPath, AppSettings _appSettings, string domain, string userName, string password)
    {
        string command = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools\\Autologon.exe");

        string param = $"-accepteula {userName} {domain} {password}";

        Tools.ProcessExecutorCmdLine(command, param, _appSettings, $"Configure AutoLogon {scriptPath}");
    }
    public static void DisableAutoLogon()
    {
        RegistryManagement.RegistryOperations.CreateRegistryEntry(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "AutoAdminLogon", "0");
    }

    //Scheduled tasks
    private static void ScheduledTasksActions(string taskName, string action, AppSettings _appSettings, bool noEmailReturnTasks = true)
    {
        string user = Environment.UserName;
        // string taskName = "RemoteCmdScriptTask";
        string command = "schtasks.exe";
        string param = "";

        if (action == "run")
            param = $"/run /tn {taskName}";

        if (action == "delete")
            param = $"/delete /tn {taskName} /f";

        Tools.ProcessExecutorCmdLine(command, param, _appSettings, $"Scheduled task Executed. -> {taskName}", noEmailReturnTasks);
    }
    public static void ScheduleBasicTaskPowerShellScript(string scriptPath, AppSettings _appSettings)
    {
        string user = Environment.UserName;

        string taskName = "RemoteCmd-ScheduleBasicTaskPowerShellScript";
        string command = "schtasks.exe";
        string param = $"/create /tn {taskName} /tr \"powershell.exe -NoProfile -ExecutionPolicy Bypass -File '{scriptPath}'\" /sc onlogon /ru {user}  /IT /f";

        ScheduledTasksActions(taskName, "delete", _appSettings);
        Tools.ProcessExecutorCmdLine(command, param, _appSettings, $"Scheduled task created. -> {scriptPath}");
        ScheduledTasksActions(taskName, "run", _appSettings);
        ScheduledTasksActions(taskName, "delete", _appSettings);
    }
    public static void ScheduleTaskHighestPowerShellScript(string scriptPath, AppSettings _appSettings, string userName, string password)
    {
        string taskName = "RemoteCmd-ScheduleTaskHighestPowerShellScript";
        string command = "schtasks.exe";
        string param = $"/create /tn {taskName} /tr  \"powershell.exe -NoProfile -ExecutionPolicy Bypass -File '{scriptPath}'\" /sc onlogon /ru {userName} /rp {password} /RL HIGHEST /IT /f";

        ScheduledTasksActions(taskName, "delete", _appSettings);
        Tools.ProcessExecutorCmdLine(command, param, _appSettings, $"Scheduled task created. -> {scriptPath}");
        ScheduledTasksActions(taskName, "run", _appSettings);
        ScheduledTasksActions(taskName, "delete", _appSettings);
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

    //     Tools.ProcessExecutorCmdLine(command, param, _appSettings, $"Scheduled task created. -> {scriptPath}");
    //     //get current user logged and save in a key on registry.
    //    // RegistryManagement.RegistryOperations.CreateRegistryEntry(Registry.LocalMachine, "SOFTWARE\\RemoteCmd", "current logged user", Environment.UserName);
    // }


    // private static bool IsProcessRunning(string processName)
    // {
    //     return Process.GetProcessesByName(processName).Length > 0;
    // }
    // public static void ScheduledExecutorElevatedAction(string scriptPath, AppSettings _appSettings)
    // {


    //     string username = "administrador"; // Substitua pelo nome de usuário do administrador
    //     string password = "http2025$"; // Substitua pela senha do administrador

    //     string taskName = "RemoteCmdScriptBasicTask";
    //     string param = $"/create /tn {taskName} /tr \"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ps1ToRun.exe")} '{scriptPath}'\" /sc onlogon /ru {username} /rp {password} /RL HIGHEST /IT /f";
    //     string command = "schtasks.exe";

    //     ScheduledTasksAction(taskName, "delete", _appSettings);
    //     Tools.ProcessExecutorCmdLine(command, param, _appSettings, $"Scheduled task created. -> {scriptPath}");
    //     ScheduledTasksAction(taskName, "run", _appSettings);
    //     ScheduledTasksAction(taskName, "delete", _appSettings);
    // }




}