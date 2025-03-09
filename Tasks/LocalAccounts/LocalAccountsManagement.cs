using System.Collections;
using System.DirectoryServices;
using System.Reflection;
using System.Security.Principal;
using Org.BouncyCastle.Utilities;
using remoteCmd.Tasks.LocalAccounts.Interfaces;
using remoteCmd.Tasks.Scripts;
public static class LocalAccountsManagement
{
    static string computerName = Environment.MachineName;
    static string pathComputer = $"WinNT://{computerName}, computer";
    static string pathComputerUser = $"WinNT://" + computerName + "/";
    static string sidDefaultUsersGroup = "S-1-5-32-545"; // SID of default users group.
    static string sidDefaultAdministratorsGroup = "S-1-5-32-544"; // SID of default Administrators group.
    private static DirectoryEntry GetDirectoryEntry(string path)
    {
        try
        {

            if (OperatingSystem.IsWindows())
                return new DirectoryEntry(path);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Internal Error - {Environment.MachineName} - {DateTime.Now}", $"Some things wrong with accounts. {ex.Message}", "", BasePath.AppSettingsJsonFile);
        }
        return null;
    }
    private static bool AddUserToGroup(string userName, string groupName = "Users")
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                var user = GetDirectoryEntry(pathComputer);
                var userEntry = user.Children.Find(userName, "User");

                var usersGroup = GetDirectoryEntry(pathComputer).Children.Find(groupName, "Group");

                if (usersGroup != null)
                {
                    var result = usersGroup.Invoke("Add", new object[] { userEntry.Path });
                    return true;
                }
                else
                    return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Password no changed for {userName} - {Environment.MachineName} - {DateTime.Now}", $"Some things wrong, password no changed: {userName} {ex.Message}", "", BasePath.AppSettingsJsonFile);
        }
        return false;
    }
    private static string FindDefaultUsersGroup(string sid)
    {

        try
        {
            if (OperatingSystem.IsWindows())
            {
                var localMachine = GetDirectoryEntry(pathComputer);

                foreach (DirectoryEntry group in localMachine.Children)
                {
                    if (group.SchemaClassName == "Group")
                    {
                        byte[] groupSIDBytes = (byte[])group.InvokeGet("objectSid");
                        if (groupSIDBytes != null)
                        {
                            string groupSID = ConvertSidToString(groupSIDBytes).ToString();
                            if (groupSID == sid)
                            {
                                return group.Name; // Grupo encontrado, encerra o loop
                            }

                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return "";
    }
    private static object ConvertSidToString(byte[] sidBytes)
    {
        if (OperatingSystem.IsWindows())
        {
            var sid = new SecurityIdentifier(sidBytes, 0);
            return sid;
        }
        return "";
    }
    private static int Flags(DirectoryEntry userEntry, string userName, bool passwordExpires = false, bool neverExpires = false, bool accountDisabled = false)
    {
        int userFlags = (int)userEntry.InvokeGet("UserFlags");

        if (OperatingSystem.IsWindows())
        {

            if (passwordExpires)
            {
                userFlags |= (int)UserFlags.ADS_UF_PASSWORD_EXPIRED;
                userFlags &= ~(int)UserFlags.ADS_UF_DONT_EXPIRE_PASSWD;
                Tools.ProcessExecutorNoWaitCmdLine("net", $"user {userName} /logonpasswordchg:yes", BasePath.AppSettingsJsonFile, string.Empty, false);
            }
            else
                userFlags &= ~(int)UserFlags.ADS_UF_PASSWORD_EXPIRED;

            if (neverExpires)
            {
                userFlags &= ~(int)UserFlags.ADS_UF_PASSWORD_EXPIRED;
                userFlags |= (int)UserFlags.ADS_UF_DONT_EXPIRE_PASSWD;
            }
            else
                userFlags &= ~(int)UserFlags.ADS_UF_DONT_EXPIRE_PASSWD;

            if (accountDisabled)
                userFlags &= ~(int)UserFlags.ADS_UF_ACCOUNTDISABLE;
            else
                userFlags |= (int)UserFlags.ADS_UF_ACCOUNTDISABLE;

        }
        return userFlags;

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
        Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"List of Users Accounts - {Environment.MachineName} - {DateTime.Now}", $"{result}", "", BasePath.AppSettingsJsonFile);
    }
    public static void ChangePassword(string userName, string password, bool passwordExpires = false, bool neverExpires = false)
    {
        try
        {
            var PasswordExpiresReturn = passwordExpires ? " - The password will expire at the next logon." : "";
            var PasswordNeverExpiresReturn = neverExpires ? " - Password never expires." : "";

            if (OperatingSystem.IsWindows())
            {
                var userEntry = GetDirectoryEntry($"WinNT://" + computerName + "/" + userName);
                userEntry.Invoke("SetPassword", new object[] { password });

                userEntry.Invoke("Put", new object[] { "UserFlags", Flags(userEntry, userName, passwordExpires, neverExpires, true) });
                userEntry.CommitChanges();

                Console.WriteLine("Password successfully changed.");

                Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName,
                    $"Password changed for {userName} - {Environment.MachineName} - {DateTime.Now}",
                    $"Password changed: {userName} new password: {password}{PasswordExpiresReturn}{PasswordNeverExpiresReturn}",
                    "", BasePath.AppSettingsJsonFile);
            }
        }
        catch (TargetInvocationException ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName,
                $"Password not changed for {userName} - {Environment.MachineName} - {DateTime.Now}",
                $"Something went wrong, password not changed: {userName} {ex.Message}",
                "", BasePath.AppSettingsJsonFile);
        }
    }
    public static void CreateLocalAccount(string userName, string password, string[] groups, bool passwordExpires = false, bool neverExpires = false)
    {
        try
        {

            if (OperatingSystem.IsWindows())
            {
                var localMachine = GetDirectoryEntry(pathComputer);
                var userEntry = localMachine.Children.Add(userName, "user");
                userEntry.Invoke("SetPassword", new object[] { password });
                userEntry.CommitChanges();
                Thread.Sleep(1000);
                var userEntryAgain = GetDirectoryEntry($"WinNT://" + computerName + "/" + userName);
                userEntryAgain.Invoke("Put", new object[] { "UserFlags", Flags(userEntry, userName, passwordExpires, neverExpires, false) });

                userEntryAgain.CommitChanges();
                Console.WriteLine("Account successfully created.");

            }
            string groupsAdded = string.Empty;
            if (groups == Array.Empty<string>())
            {
                AddUserToGroup(userName, FindDefaultUsersGroup(sidDefaultUsersGroup));
                groupsAdded = FindDefaultUsersGroup(sidDefaultUsersGroup);
            }
            else
            {
                foreach (var group in groups)
                {
                    AddUserToGroup(userName, group);
                    groupsAdded += " - " + group;
                }
            }
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Account successfully created. {userName} - {Environment.MachineName} - {DateTime.Now}", $"{userName} was added to groups: {groupsAdded}", "", BasePath.AppSettingsJsonFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Failed to create account {userName} - {Environment.MachineName} - {DateTime.Now}", $"Some things wrong: {userName} {ex.Message}", "", BasePath.AppSettingsJsonFile);
        }
    }
    public static void EnableDisableAccount(string userName, bool disableEnable)
    {
        string actionReturn = disableEnable ? "enabled":"disabled";

        try
        {
            if (OperatingSystem.IsWindows())
            {
                var localMachineUser = GetDirectoryEntry(pathComputerUser + userName);

                localMachineUser.Invoke("Put", new object[] { "UserFlags", Flags(localMachineUser, userName, false, false, disableEnable) });

                localMachineUser.CommitChanges();

                Console.WriteLine("Account {actionReturn} successfully.");

                Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Account {actionReturn} successfully. {userName} - {Environment.MachineName} - {DateTime.Now}", $"{userName} was {actionReturn}.", "", BasePath.AppSettingsJsonFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Failed when try {actionReturn} account. {userName} - {Environment.MachineName} - {DateTime.Now}", $"Some things wrong: {userName} {ex.Message}", "", BasePath.AppSettingsJsonFile);
        }
    }
}