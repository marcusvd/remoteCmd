using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
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

                var path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\ScriptsToExecute");
                if (!Directory.Exists(path1))
                    Directory.CreateDirectory(path1);

                var path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs");
                if (!Directory.Exists(path2))
                    Directory.CreateDirectory(path2);


                LastExecution.ServiceConf.LastExecution = uniqueId;

                var path = JsonManagement.jsonPath;

                JsonManagement.JsonWrite(path ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSettings.json"), LastExecution);

                var singleMessage = inbox.GetMessage(msg.Index);

                if (singleMessage.Attachments.Any())
                {
                    try
                    {
                        foreach (var item in singleMessage.Attachments)
                        {
                            if (item is MimePart mimePart)
                            {
                                var fileName = mimePart.FileName;

                                var pathScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\ScriptsToExecute");

                                var filePath = Path.Combine(pathScript, fileName);

                                // Salvar o anexo
                                using (var stream = File.Create(filePath))
                                {
                                    mimePart.Content.DecodeTo(stream);
                                }

                                Basics.ScheduledTasksElevatedAction(filePath, _appSettings);
                                // Basics.ScheduleBasicTask(filePath, _appSettings);
                                
                                //  Basics.ExecutePowerShellScript(filePath);

                                Console.WriteLine($"attachment saved in: {filePath}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry("RemoteCmd", $"{ex.ToString()} attached .ps1 file not found.", EventLogEntryType.Error);
                    }
                }
                else
                    Executions.ExecutionsToExecute(singleMessage.Body.ToString(), _appSettings);

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