using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Web;
using MailKit;
using MimeKit;
using PasswordManagement;


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

        var computers = GetCumpotersExecution(msg.Envelope.Subject);

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


        var LastExecution = JsonManagement.LoadJson(pathJson);


        if (uniqueId > LastExecution.ServiceConf.LastExecution)
        {

            if (checkGroup && computerToExecute)
            {

                LastExecution.ServiceConf.LastExecution = uniqueId;

                var path = JsonManagement.jsonPath;

                JsonManagement.JsonWrite(path ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSettings.json"), LastExecution);

                var singleMessage = inbox.GetMessage(msg.Index);

                CallToExecute call = new CallToExecute(_appSettings, singleMessage);

                if (singleMessage.Attachments.Any())
                    call.ScriptAttachmentsToExecute();
                else
                {
                    Executions.ActionPreDefinedsToExecute(singleMessage.Body.ToString(), _appSettings);

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
        var code = subject.Split(',')[0].Trim();
        if (!string.IsNullOrEmpty(code))
            return code;
        else
            return "";
    }

    public string[] GetGroupsExecution(string subject)
    {
        string pattern = @"\(([^)]+)\)";
        Match match = Regex.Match(subject, pattern);
        if (match.Success)
        {
            string items = match.Groups[1].Value;
            string[] itemsArray = items.Split(',');
            return itemsArray;
        }
        else
            return [];
    }

    public string[] GetCumpotersExecution(string subject)
    {
        string[] parts = subject.Split(',');
        var itemsWithHyphen = new List<string>();
        foreach (string part in parts)
        {
            if (part.Trim().StartsWith("-"))
            {
                itemsWithHyphen.Add(part.Replace("-", "").Trim());
            }
        }
        return itemsWithHyphen.ToArray();
    }

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