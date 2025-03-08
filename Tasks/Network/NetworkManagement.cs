using System.Diagnostics;
using Microsoft.Win32;
using remoteCmd.Tasks.RegistryOperations;
using remoteCmd.Tasks.Scripts;
namespace remoteCmd.Tasks.Network;
public static class NetworkManagement
{
    public static void GetIpAll(AppSettings _appSettings)
    {
        string command = "powershell";
        string param = "Get-NetIPConfiguration -All";
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings);
    }
    public static void FirewallEnableDisable(bool enableDisable, AppSettings _appSettings)
    {
        string param;
        string command = "netsh";
        if (enableDisable)
            param = "advfirewall set allprofiles state on";
        else
            param = "advfirewall set allprofiles state off";
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, enableDisable ? "Firewall - successfully enabled." : "Firewall successfully disabled.");
    }
    public static void GetFirewallState(AppSettings _appSettings)
    {
        string command = "netsh";
        string param = "advfirewall show allprofiles";

        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Firewall state now:");
    }
    public static void RemoteDesktopEnableDisable(int enableDisable, AppSettings _appSettings)
    {
        string dDeny = "System\\CurrentControlSet\\Control\\Terminal Server";
        string userAuth = "System\\CurrentControlSet\\Control\\Terminal Server\\WinStations\\RDP-Tcp";

        if (enableDisable == 0)
        {
            RegistryManagement.CreateRegistryEntry(Registry.LocalMachine, dDeny, "fDenyTSConnections", enableDisable);
            RegistryManagement.CreateRegistryEntry(Registry.LocalMachine, userAuth, "UserAuthentication", enableDisable);
            Sender.SendEmail(_appSettings.ServerSmtp.UserName, $" Remote desktop. - {Environment.MachineName} - {DateTime.Now}", $"Remote desktop successfully enabled.", "", _appSettings);
        }
        if (enableDisable != 0)
        {
            RegistryManagement.CreateRegistryEntry(Registry.LocalMachine, dDeny, "fDenyTSConnections", enableDisable);
            RegistryManagement.CreateRegistryEntry(Registry.LocalMachine, userAuth, "UserAuthentication", enableDisable);
            Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Remote desktop. - {Environment.MachineName} - {DateTime.Now}", $"Remote desktop successfully disabled.", "", _appSettings);
        }
       
    }
    

}