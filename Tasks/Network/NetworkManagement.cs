using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;
using Org.BouncyCastle.Utilities;
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

    public static void SendWakeOnLan(string macAddress, AppSettings _appSettings)
    {
        try{

        
      // Remove os separadores do endereço MAC (se houver)
        macAddress = macAddress.Replace(":", "").Replace("-", "");

        // Converte o endereço MAC em um array de bytes
        byte[] macBytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            macBytes[i] = Convert.ToByte("0AE0AFC1047F".Substring(i * 2, 2), 16);
            // macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16);
        }

        // Cria o pacote mágico
        byte[] magicPacket = new byte[102];
        for (int i = 0; i < 6; i++)
        {
            magicPacket[i] = 0xFF;
        }
        for (int i = 1; i <= 16; i++)
        {
            Array.Copy(macBytes, 0, magicPacket, i * 6, 6);
        }

        // Envia o pacote mágico via UDP para o endereço de broadcast
        using (UdpClient client = new UdpClient())
        {
            client.Connect(IPAddress.Broadcast, 9);
            client.Send(magicPacket, magicPacket.Length);
        }
        }
        catch(Exception ex){
            Console.WriteLine(ex.Message);
        }
        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"WakeOnLan - {Environment.MachineName} - {DateTime.Now}", $"WakeOnLan Magic Packet was successfully sent. to {IPAddress.Broadcast} to MacAddress: {macAddress}", "", _appSettings);
    }


}