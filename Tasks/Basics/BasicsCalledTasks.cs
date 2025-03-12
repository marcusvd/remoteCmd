
namespace remoteCmd.Tasks.Basic;
public class BasicsCalledTasks
{

    public static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
    {
        Console.WriteLine($"Body: {body}");

        var shutdown = body.Contains("shutdown", StringComparison.OrdinalIgnoreCase);
        if (shutdown)
            BasicsManagement.Shutdown(_appSettings);

        var logoff = body.Contains("logoff", StringComparison.OrdinalIgnoreCase);
        if (logoff)
            BasicsManagement.Logoff(_appSettings);

        var reboot = body.Contains("reboot", StringComparison.OrdinalIgnoreCase);
        if (reboot)
            BasicsManagement.Reboot(_appSettings);
       
        var documentation = body.Contains("documentation", StringComparison.OrdinalIgnoreCase);
        if (documentation)
            BasicsManagement.Documentation(_appSettings);
    }


}