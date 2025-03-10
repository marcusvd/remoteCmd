namespace remoteCmd.Tasks.Network;

public static class NetworkCalledTasks
{
    public static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
    {
        var getIpAll = body.Contains("getIpAll", StringComparison.OrdinalIgnoreCase);
        if (getIpAll)
            NetworkManagement.GetIpAll(_appSettings);

        var firewallDisable = body.Contains("firewallDisable", StringComparison.OrdinalIgnoreCase);
        if (firewallDisable)
            NetworkManagement.FirewallEnableDisable(false, _appSettings);

        var firewallEnable = body.Contains("firewallEnable", StringComparison.OrdinalIgnoreCase);
        if (firewallEnable)
            NetworkManagement.FirewallEnableDisable(true, _appSettings);

        var getFirewallState = body.Contains("getFirewallState", StringComparison.OrdinalIgnoreCase);
        if (getFirewallState)
            NetworkManagement.GetFirewallState(_appSettings);

        var RemoteDesktopEnable = body.Contains("RemoteDesktopEnable", StringComparison.OrdinalIgnoreCase);
        if (RemoteDesktopEnable)
            NetworkManagement.RemoteDesktopEnableDisable(0, _appSettings);

        var RemoteDesktopDisable = body.Contains("RemoteDesktopDisable", StringComparison.OrdinalIgnoreCase);
        if (RemoteDesktopDisable)
            NetworkManagement.RemoteDesktopEnableDisable(1, _appSettings);

        var WakeOnLan = body.Contains("WakeOnLan", StringComparison.OrdinalIgnoreCase);
        if (WakeOnLan)
        {
            var getMac = body.Split("|");
            var handleToGetMac = getMac.FirstOrDefault(mac => mac.StartsWith("Mac:", StringComparison.OrdinalIgnoreCase));


            if (handleToGetMac != null)
            {
                int startIndex = handleToGetMac.IndexOf('[') + 1;
                int endIndex = handleToGetMac.IndexOf(']');
                string listMacs = handleToGetMac.Substring(startIndex, endIndex - startIndex);
                var macs = listMacs.Split('#');
                if (macs.Length > 1)
                    Console.WriteLine(macs[0]);

                NetworkManagement.SendWakeOnLan(macs[0].Trim(), _appSettings);
            }
        }
    }



}
