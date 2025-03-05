using System.Collections;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Configuration.UserSecrets;
public static class LocalAccountsManagement
{
    static string computerName = Environment.MachineName;
    static string pathComputer = $"WinNT://{computerName}, computer";
    private static DirectoryEntry GetDirectoryEntry(string path)
    {
        if (OperatingSystem.IsWindows())
            return new DirectoryEntry(path);

        return null;
    }

    public static void GetAllLocalAccounts()
    {

        string result = string.Empty;

        if (OperatingSystem.IsWindows())
        {
            result += TextFile.OutPut("==================User And Members Of Group================");

            foreach (DirectoryEntry entry in GetDirectoryEntry(pathComputer).Children)
            {
                if (entry.SchemaClassName == "User")
                {
                    result += TextFile.OutPut($"{entry.Name}");
                    result += TextFile.OutPut("Grupos:");

                    var groups = entry.Invoke("Groups");
                    if (groups is IEnumerable)
                    {
                        foreach (object group in (IEnumerable)groups)
                        {
                            using (DirectoryEntry groupEntry = new DirectoryEntry(group))
                            {
                                result += TextFile.OutPut($" - {groupEntry.Name}");
                            }
                        }
                    }
                    result += TextFile.OutPut(new string('=', 60));
                }
            }
        }

        Console.WriteLine(result);
    }

    public static void ChangePassword(string userName, string password)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                var userEntry = GetDirectoryEntry($"WinNT://{computerName} + /{userName}");
                userEntry.Invoke("SetPassword", new object[] { password });
                userEntry.CommitChanges();
                Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Password changed for {userName} - {Environment.MachineName} - {DateTime.Now}", $"Password changed: {userName} new password: {password}", "", BasePath.AppSettingsJsonFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Password no changed for {userName} - {Environment.MachineName} - {DateTime.Now}", $"Some things wrong, password no changed: {userName} {ex.Message}", "", BasePath.AppSettingsJsonFile);
        }
    }




}