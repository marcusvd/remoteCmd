using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration.UserSecrets;
public class LocalAccounts
{

    private static PrincipalContext PrincipalContext()
    {
        try
        {
            if (OperatingSystem.IsWindows()) return new PrincipalContext(ContextType.Machine);
            else return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Management Account error unknown. - {Environment.MachineName} - {DateTime.Now}", $"Error:{ex.Message}", "", BasePath.AppSettingsJsonFile);
            return null;
        }

    }
    private static UserPrincipal FindByUserName(PrincipalContext context, string userName)
    {
        try
        {
            if (OperatingSystem.IsWindows()) return UserPrincipal.FindByIdentity(context, userName);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Some thing wrong when find user - {Environment.MachineName} - {DateTime.Now}", $"Error:{ex.Message}", "", BasePath.AppSettingsJsonFile);
            return null;
        }
    }
    private static GroupPrincipal FindGroup(string groupName)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                var principalContext = PrincipalContext();
                var group = GroupPrincipal.FindByIdentity(principalContext, groupName);
                return group;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Some thing wrong when find group - {Environment.MachineName} - {DateTime.Now}", $"Error:{ex.Message}", "", BasePath.AppSettingsJsonFile);
            return null;
        }
        return null;
    }
    private static GroupPrincipal AddUserToGroup(string groupName, string userName)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                var principalContext = PrincipalContext();
                var user = FindByUserName(principalContext, userName);
                var group = FindGroup(groupName);

                if (user != null && group != null)
                {
                    group.Members.Add(user);
                    group.Save();
                    Console.WriteLine($"User {userName} has been added to the group {groupName}.");
                    Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Group Account added - {Environment.MachineName} - {DateTime.Now}", $"User {userName} has been added to the group {groupName}.", "", BasePath.AppSettingsJsonFile);
                }
                else
                {
                    Console.WriteLine("User or group not found");
                    Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Group Account wrong - {Environment.MachineName} - {DateTime.Now}", $"Error:User or group not found", "", BasePath.AppSettingsJsonFile);
                }

                return group;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Some thing wrong when find group - {Environment.MachineName} - {DateTime.Now}", $"Error:{ex.Message}", "", BasePath.AppSettingsJsonFile);
            return null;
        }
        return null;
    }
    public static void ResetPasswordLocalAccount(string userName, string newPassword)
    {
        try
        {
            var context = PrincipalContext();
            var user = FindByUserName(context, userName);

            if (OperatingSystem.IsWindows())
            {
                if (user != null)
                {
                    user.SetPassword(newPassword);
                    user.Save();
                    Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Password changed for {userName} - {Environment.MachineName} - {DateTime.Now}", $"Password changed: {userName} new password: {newPassword}", "", BasePath.AppSettingsJsonFile);
                }
                else
                    Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Management Account error unknown. - {Environment.MachineName} - {DateTime.Now}", $"Error:user account was null.", "", BasePath.AppSettingsJsonFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Password no changed for {userName} - {Environment.MachineName} - {DateTime.Now}", $"Some this wrong, password no changed: {userName} {ex.Message}", "", BasePath.AppSettingsJsonFile);
        }

    }
    public static void CreateLocalAccount(string userName, string password)
    {

        try
        {
            if (OperatingSystem.IsWindows())
            {
                var context = PrincipalContext();
                var newUser = new UserPrincipal(context);
                if (newUser != null)
                {
                    newUser.SamAccountName = userName;
                    newUser.SetPassword(password);
                    newUser.Enabled = true;
                    newUser.Save();
                    AddUserToGroup("", userName);
                    Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Account created - {Environment.MachineName} - {DateTime.Now}", $"Account created user name:{userName} password: {password}", "", BasePath.AppSettingsJsonFile);
                }
                else
                {
                    Console.WriteLine("Error creating user.");
                    Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Error creating {userName} - {Environment.MachineName} - {DateTime.Now}", $"Some things wrong, when creating: {userName}", "", BasePath.AppSettingsJsonFile);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Error creating {userName} - {Environment.MachineName} - {DateTime.Now}", $"Some things wrong, when creating: {userName} {ex.Message}", "", BasePath.AppSettingsJsonFile);
        }

    }
    // public static void MarkUserAccountPasswordExpiresNextLogon(string userName)
    // {

    //     try
    //     {
    //         //Set-LocalUser -UserMayChangePassword $true
    //         Tools.ProcessExecutorCmdLine("powershell","", BasePath.AppSettingsJsonFile);
    //     }
    // }

    // public static void MarkUserAccountPasswordExpiresNextLogon(string userName)
    // {

    //     try
    //     {
    //         if (OperatingSystem.IsWindows())
    //         {
    //             var context = PrincipalContext();
    //             var user = FindByUserName(context, userName);
    //             if (user != null)
    //             {
    //                 DirectoryEntry userEntry = (user.GetUnderlyingObject() as DirectoryEntry);

    //                 if (userEntry != null)
    //                 {
    //                     // Carrega explicitamente o atributo pwdLastSet
    //                     userEntry.RefreshCache(new string[] { "pwdLastSet" });

    //                     // Define o atributo pwdLastSet como 0 para forçar a expiração da senha
    //                     userEntry.Properties["pwdLastSet"].Value = 0;
    //                     userEntry.CommitChanges();
    //                 }
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //         Sender.SendEmail(BasePath.AppSettingsJsonFile.ServerSmtp.UserName, $"Error creating {userName} - {Environment.MachineName} - {DateTime.Now}", $"Some things wrong, when creating: {userName} {ex.Message}", "", BasePath.AppSettingsJsonFile);
    //     }

    // }

   

}