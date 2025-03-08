using System.Diagnostics;
using Microsoft.Win32;
using RegistryManagement;
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
        string command = "powershell.exe";
        string param = "-Command \"(Get-NetFirewallProfile | Select-Object Name, Enabled)\"";

        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Firewall state now:");
    }
    public static void RemoteDesktopEnableDisable(int enableDisable, AppSettings _appSettings)
    {
        string dDeny = "System\\CurrentControlSet\\Control\\Terminal Server";
        string userAuth = "System\\CurrentControlSet\\Control\\Terminal Server\\WinStations\\RDP-Tcp";



        if (enableDisable == 0)
        {
            CreateRegistryEntry(Registry.LocalMachine, dDeny, "fDenyTSConnections", enableDisable);
            CreateRegistryEntry(Registry.LocalMachine, userAuth, "UserAuthentication", enableDisable);
            Sender.SendEmail(_appSettings.ServerSmtp.UserName, $" Remote desktop. - {Environment.MachineName} - {DateTime.Now}", $"Remote desktop successfully enabled.", "", _appSettings);
        }
        if (enableDisable != 0)
        {
            CreateRegistryEntry(Registry.LocalMachine, dDeny, "fDenyTSConnections", enableDisable);
            CreateRegistryEntry(Registry.LocalMachine, userAuth, "UserAuthentication", enableDisable);
            Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Remote desktop. - {Environment.MachineName} - {DateTime.Now}", $"Remote desktop successfully disabled.", "", _appSettings);
        }
        // if (enableDisable == 0)
        // {
        //     param = "Set-ItemProperty -Path 'HKLM:System\\CurrentControlSet\\Control\\Terminal Server' -Name 'fDenyTSConnections' -Value 0";
        //     string param2 = "Set-ItemProperty -Path 'HKLM:System\\CurrentControlSet\\Control\\Terminal Server\\WinStations\\RDP-Tcp' -Name 'UserAuthentication' -Value 0";

       //   Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Remote desktop successfully enabled.");
        //     Tools.ProcessExecutorNoWaitCmdLine(command, param2, _appSettings, "User authentication level disabled.");
        // }
        // if (enableDisable != 0)
        // {
        //     param = "Set-ItemProperty -Path 'HKLM:System\\CurrentControlSet\\Control\\Terminal Server' -Name 'fDenyTSConnections' -Value 1";
        //     string param2 = "Set-ItemProperty -Path 'HKLM:System\\CurrentControlSet\\Control\\Terminal Server\\WinStations\\RDP-Tcp' -Name 'UserAuthentication' -Value 1";

        //     Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Remote desktop successfully disabled.");
        //     Tools.ProcessExecutorNoWaitCmdLine(command, param2, _appSettings, "User authentication level enabled.");

        // }
    }
    public static void CreateRegistryEntry(RegistryKey root, string key, string name, dynamic value)
    {
        RegistryKey registryKey = registryKeyCheck(root);
        RegistryKey registryKey2 = registryKey.CreateSubKey(key);
        if (registryKey != null)
        {
            registryKey2.SetValue(name, value);
            registryKey2.Close();
        }
    }

    private static RegistryKey registryKeyCheck(RegistryKey root)
    {
        if (root == Registry.LocalMachine)
        {
            return Registry.LocalMachine;
        }

        if (root == Registry.LocalMachine)
        {
            return Registry.CurrentUser;
        }

        return root;
    }


}