using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using MailKit;
using Microsoft.Win32;
using PasswordManagement;
using remoteCmd.Tasks.Basic;
using remoteCmd.Tasks.LocalAccounts;
using remoteCmd.Tasks.Network;
using remoteCmd.Tasks.RegistryOperations;
using remoteCmd.Tasks.Reports;
using remoteCmd.Tasks.Scripts;


public class Conditions
{
    private readonly AppSettings _appSettings;
    public Conditions(AppSettings Appsettings)
    {
        _appSettings = Appsettings;
    }

    public void ConditionsToExecute(IMessageSummary msg, int uniqueId, IMailFolder inbox, AppSettings _appSettings, string pathJson)
    {
        var groups = GetGroupsExecution(msg.Envelope.Subject);

        var computers = GetComputersExecution(msg.Envelope.Subject);

        string groupToExecution = String.Empty;

        foreach (var item in groups)
        {
            if (item == _appSettings.ParamsExecution.GroupExecution)
                groupToExecution = item;
        }

        bool computerFound = false;
        foreach (var computer in computers)
        {
            if (GetComputerIp(computer.ToLower()) || GetComputerName() == computer.ToLower())
            {
                computerFound = true;
                break;
            }
            else if (computer.ToLower() == "everyone")
            {
                computerFound = true;
                break;
            }
        }

        var checkGroup = !String.IsNullOrEmpty(groupToExecution);
        var computerToExecute = computerFound;


        // var LastExecution = JsonManagement.LoadJson(pathJson);
        var LastExecution = int.Parse(RegistryManagement.GetRegistryValue(Registry.LocalMachine, "SOFTWARE\\RemoteCmd", "LastExecution"));


        if (uniqueId > LastExecution)
        {

            if (checkGroup && computerToExecute)
            {

                LastExecution = uniqueId;
                // string last = uniqueId.ToString();
                // var path = JsonManagement.jsonPath;

                // JsonManagement.JsonWrite(path ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSettings.json"), LastExecution);
                RegistryManagement.CreateRegistryEntry(Registry.LocalMachine, "SOFTWARE\\RemoteCmd", "LastExecution", LastExecution);

                var singleMessage = inbox.GetMessage(msg.Index);

                CallToExecute call = new CallToExecute(_appSettings, singleMessage);
                if (singleMessage.Attachments.Any())
                {
                    Tools.ProcessExecutorNoWaitCmdLine("powershell", "Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force", _appSettings, "Execution policy was set to Unrestricted", false);
                    call.ScriptAttachmentsToExecute();
                }
                else
                {
                    BasicsCalledTasks.ActionPreDefinedsToExecute(singleMessage.Body.ToString(), _appSettings);
                    CalledAccountTasks.ActionPreDefinedsToExecute(singleMessage.Body.ToString(), _appSettings);
                    WindowsLogsCalledTasks.ActionPreDefinedsToExecute(singleMessage.Body.ToString(), _appSettings);
                    SoftwareCalledTasks.ActionPreDefinedsToExecute(singleMessage.Body.ToString(), _appSettings);
                    NetworkCalledTasks.ActionPreDefinedsToExecute(singleMessage.Body.ToString(), _appSettings);
                    HardwareCalledTasks.ActionPreDefinedsToExecute(singleMessage.Body.ToString(), _appSettings);

                    // if (singleMessage.Body.ToString().Contains("PowershellScriptRun"))
                    // {
                    //       Basics.PowershellScriptRun(command, _appSettings);
                    // }
                }
            }
        }

    }

    public string GetCodeExecution(string subject)
    {
        var splitSubject = subject.Split('=');
        var checkContainsCode = splitSubject.FirstOrDefault(x => x.StartsWith("code:", StringComparison.OrdinalIgnoreCase));
        if (checkContainsCode != null)
        {
            int startIndex = checkContainsCode.IndexOf('[') + 1;
            int endIndex = checkContainsCode.IndexOf(']');
            string code = checkContainsCode.Substring(startIndex, endIndex - startIndex);

            return code;
        }

        return string.Empty;
    }
    // public string GetCodeExecution(string subject)
    // {
    //     var code = subject.Split(',')[0].Trim();
    //     if (!string.IsNullOrEmpty(code))
    //         return code;
    //     else
    //         return "";
    // }

    public string[] GetGroupsExecution(string subject)
    {
        var splitSubject = subject.Split('=');
        var checkContainsGroups = splitSubject.FirstOrDefault(x => x.StartsWith("groups:", StringComparison.OrdinalIgnoreCase));
        if (checkContainsGroups != null)
        {
            int startIndex = checkContainsGroups.IndexOf('[') + 1;
            int endIndex = checkContainsGroups.IndexOf(']');
            string groups = checkContainsGroups.Substring(startIndex, endIndex - startIndex);

            return groups.Split(',');
        }

        return Array.Empty<string>();
    }
    // public string[] GetGroupsExecution(string subject)
    // {
    //     string pattern = @"\(([^)]+)\)";
    //     Match match = Regex.Match(subject, pattern);
    //     if (match.Success)
    //     {
    //         string items = match.Groups[1].Value;
    //         string[] itemsArray = items.Split(',');
    //         return itemsArray;
    //     }
    //     else
    //         return [];
    // }

    public string[] GetComputersExecution(string subject)
    {
       var splitSubject = subject.Split('=');
        var checkContainsTargets = splitSubject.FirstOrDefault(x => x.StartsWith("targets:", StringComparison.OrdinalIgnoreCase));
        if (checkContainsTargets != null)
        {
            int startIndex = checkContainsTargets.IndexOf('[') + 1;
            int endIndex = checkContainsTargets.IndexOf(']');
            string Targets = checkContainsTargets.Substring(startIndex, endIndex - startIndex);

            return Targets.Split(',');
        }

        return Array.Empty<string>();
    }
    // public string[] GetCumpotersExecution(string subject)
    // {
    //     string[] parts = subject.Split(',');
    //     var itemsWithHyphen = new List<string>();
    //     foreach (string part in parts)
    //     {
    //         if (part.Trim().StartsWith("-"))
    //         {
    //             itemsWithHyphen.Add(part.Replace("-", "").Trim());
    //         }
    //     }
    //     return itemsWithHyphen.ToArray();
    // }

    public bool CheckCodeExecution(string code)
    {
        var codeDecrypted = PasswordManager.Decrypt(_appSettings.ParamsExecution.SecretExecutionCode);

        if (code == codeDecrypted)
            return true;
        else
            return false;
    }
    public string GetComputerName()
    {
        return Environment.MachineName.ToLower();
    }
    public bool GetComputerIp(string ipAddress)
    {
        try
        {
            IPAddress targetIp = IPAddress.Parse(ipAddress);

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.Equals(targetIp))
                    {
                        if (ni.Name != null || ni.Name != string.Empty)
                            return true;
                    }
                }
            }
        }
        catch (Exception)
        {
            return false;
        }


        return false;
    }


}