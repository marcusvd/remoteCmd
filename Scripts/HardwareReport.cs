using System.Management;
using System.Text;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

public class HardwareReport
{
    private const ulong OneGB = 1024 * 1024 * 1024;
    private const ulong OneTB = OneGB * 1024;

    public static async Task<string> GetHardwareReportAsync()
    {
        StringBuilder report = new StringBuilder();

        report.AppendLine("Computer Hardware Report");
        report.AppendLine(new string('=', 50));
        report.AppendLine();

        // Informações da CPU
        report.AppendLine("CPU Information:");
        report.AppendLine(await GetCPUInfoAsync());
        report.AppendLine();

        // Informações da Memória
        report.AppendLine("Memory Information:");
        report.AppendLine(await GetMemoryInfoAsync());
        report.AppendLine();

        // Informações do Disco Rígido
        report.AppendLine("Hard Drive Information:");
        report.AppendLine(await GetDiskDriveInfoAsync());
        report.AppendLine();

        // Informações da Placa de Vídeo
        report.AppendLine("Video Card Information:");
        report.AppendLine(await GetVideoInfoAsync("Win32_VideoController", "Name", "AdapterRAM"));
        report.AppendLine();

        // Informações da Placa Mãe
        report.AppendLine("Motherboard Information:");
        report.AppendLine(await GetHardwareInfoAsync("Win32_BaseBoard", "Manufacturer", "Product"));
        report.AppendLine();

        // Informações da Placa de Rede
        report.AppendLine("Network Adapter Information:");
        report.AppendLine(await GetNetworkAdapterInfoAsync());
        report.AppendLine();

        // Exibir relatório
        return report.ToString();
    }

    private static async Task<string> GetHardwareInfoAsync(string className, params string[] properties)
    {
        StringBuilder info = new StringBuilder();

        try
        {
            var results = await QueryWmiAsync(className);

            foreach (var obj in results)
            {
                foreach (string property in properties)
                {
                    info.AppendLine($"{property}: {obj[property]}");
                }
                info.AppendLine(new string('-', 50));
            }
        }
        catch (Exception ex)
        {
            info.AppendLine($"Erro ao obter informações de {className}: {ex.Message}");
        }

        return info.ToString();
    }

    private static async Task<string> GetVideoInfoAsync(string className, params string[] properties)
    {
        StringBuilder info = new StringBuilder();

        try
        {
            var results = await QueryWmiAsync(className);

            foreach (var obj in results)
            {
                foreach (string property in properties)
                {
                    if (property == "AdapterRAM" && obj[property] != null)
                    {
                        uint adapterRamBytes = (uint)obj[property];
                        double adapterRamGB = Math.Round((double)adapterRamBytes / OneGB, 2);
                        info.AppendLine($"AdapterRAM: {adapterRamGB} GB");
                    }
                    else
                    {
                        info.AppendLine($"{property}: {obj[property]}");
                    }
                }
                info.AppendLine(new string('-', 50));
            }
        }
        catch (Exception ex)
        {
            info.AppendLine($"Erro ao obter informações de {className}: {ex.Message}");
        }

        return info.ToString();
    }

    private static async Task<string> GetCPUInfoAsync()
    {
        StringBuilder info = new StringBuilder();

        try
        {
            var results = await QueryWmiAsync("Win32_Processor");

            foreach (var obj in results)
            {

                info.AppendLine($"Name: {obj["Name"]}");
                info.AppendLine($"NumberOfCores: {obj["NumberOfCores"]}");

                uint maxClockSpeedMHz = (uint)obj["MaxClockSpeed"];
                double maxClockSpeedGHz = Math.Round(maxClockSpeedMHz / 1000.0, 2);
                info.AppendLine($"MaxClockSpeed: {maxClockSpeedGHz} GHz");
                info.AppendLine(new string('-', 50));

            }
        }
        catch (Exception ex)
        {
            info.AppendLine($"Erro ao obter informações da CPU: {ex.Message}");
        }

        return info.ToString();
    }

    private static async Task<string> GetMemoryInfoAsync()
    {
        StringBuilder info = new StringBuilder();

        try
        {
            var results = await QueryWmiAsync("Win32_PhysicalMemory");

            foreach (var obj in results)
            {
                ulong capacityBytes = (ulong)obj["Capacity"];
                double capacityGB = Math.Round((double)capacityBytes / OneGB, 2);
                info.AppendLine($"Capacity: {capacityGB} GB");

                string speed = obj["Speed"]?.ToString() ?? "N/A";
                info.AppendLine($"Speed: {speed} MHz");
                info.AppendLine(new string('-', 50));
            }
        }
        catch (Exception ex)
        {
            info.AppendLine($"Erro ao obter informações de memória: {ex.Message}");
        }

        return info.ToString();
    }

    private static async Task<string> GetDiskDriveInfoAsync()
    {
        StringBuilder info = new StringBuilder();
        DriveInfo[] allDrives = DriveInfo.GetDrives();

        foreach (DriveInfo d in allDrives)
        {
            info.AppendLine($"Drive {d.Name}");
            info.AppendLine($"  Drive type: {d.DriveType}");
            if (d.IsReady)
            {
                info.AppendLine($"  Volume label: {d.VolumeLabel}");
                info.AppendLine($"  File system: {d.DriveFormat}");
                info.AppendLine($"  Available space to current user: {FormatSize(d.AvailableFreeSpace)}");
                info.AppendLine($"  Total available space: {FormatSize(d.TotalFreeSpace)}");
                info.AppendLine($"  Total size of drive: {FormatSize(d.TotalSize)}");
                string diskType = await GetDiskTypeAsync(d.Name);
                info.AppendLine($"  Disk type: {diskType}");
            }
        }
        return info.ToString();
    }

    private static string FormatSize(long bytes)
    {
        double sizeInGB = bytes / (1024.0 * 1024.0 * 1024.0);
        if (sizeInGB >= 1024)
        {
            double sizeInTB = sizeInGB / 1024.0;
            return $"{sizeInTB:F2} TB";
        }
        else
        {
            return $"{sizeInGB:F2} GB";
        }
    }

    private static async Task<string> GetDiskTypeAsync(string driveName)
    {
        return await Task.Run(() =>
        {
            string diskType = "Unknown";
            try
            {
                string query = "SELECT MediaType FROM Win32_DiskDrive";
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject drive in searcher.Get())
                    {
                        if (drive["MediaType"] != null)
                        {
                            diskType = drive["MediaType"].ToString();
                            if (diskType.Contains("SSD"))
                            {
                                return "SSD";
                            }
                            else if (diskType.Contains("HDD"))
                            {
                                return "HDD (SATA)";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                diskType = $"Error: {ex.Message}";
            }
            return diskType;
        });
    }

    private static async Task<string> GetNetworkAdapterInfoAsync()
    {
        StringBuilder info = new StringBuilder();

        try
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    info.AppendLine($"Name: {ni.Name}");
                    info.AppendLine($"Description: {ni.Description}");
                    info.AppendLine($"Physical Address: {ni.GetPhysicalAddress()}");

                    IPInterfaceProperties ipProperties = ni.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            info.AppendLine($"IP Address: {ip.Address}");
                            info.AppendLine($"Subnet Mask: {ip.IPv4Mask}");
                        }
                    }

                    foreach (GatewayIPAddressInformation gateway in ipProperties.GatewayAddresses)
                    {
                        info.AppendLine($"Gateway: {gateway.Address}");
                    }

                    foreach (IPAddress dns in ipProperties.DnsAddresses)
                    {
                        info.AppendLine($"DNS: {dns}");
                    }

                    info.AppendLine(new string('-', 50));
                }
            }
        }
        catch (Exception ex)
        {
            info.AppendLine($"Erro ao obter informações de rede: {ex.Message}");
        }

        return info.ToString();
    }

    private static async Task<ManagementObjectCollection> QueryWmiAsync(string className)
    {
        return await Task.Run(() =>
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM {className}"))
            {
                return searcher.Get();
            }
        });
    }
   

}
