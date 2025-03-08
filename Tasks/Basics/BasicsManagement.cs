// using System;
// using System.Diagnostics;
// using System.Drawing;
// using System.Drawing.Imaging;
// using System.Windows.Forms;

using remoteCmd.Tasks.Scripts;

namespace remoteCmd.Tasks.Basic;
public class BasicsManagement
{

    public static void Shutdown(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/s /t 15";
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Shutdown in 15 seconds.");
    }
    public static void Logoff(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/l";
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Logoff action executed.");
    }
    public static void Reboot(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/r /t 15";
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Reboot in 15 seconds.");
    }
}