using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using MailKit;
using MimeKit;

public class Conditions
{
    private readonly Appsettings _Appsettings;
    public Conditions(Appsettings Appsettings)
    {
        _Appsettings = Appsettings;
    }

    public void ConditionsToExecute(IMessageSummary msg, int uniqueId, IMailFolder inbox, Appsettings _appSettingsJson, string pathJson)
    {
        // var codeExecution = GetCodeExecution(lastMsg.Subject);

        var groups = GetGroupsExecution(msg.Envelope.Subject);

        var computers = GetCumpotersExecution(msg.Envelope.Subject);


        string groupToExecution = String.Empty;

        foreach (var item in groups)
        {
            if (item == _Appsettings.ParamsExecution.GroupExecution)
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

        // var checkedCodeExecution = CheckCodeExecution(codeExecution);
        var checkGroup = !String.IsNullOrEmpty(groupToExecution);
        var computerToExecute = computerFound;

        // Console.WriteLine($"RESULT: {checkedCodeExecution}, {checkGroup}, {computerToExecute}");
       // var _pathJson = new JsonOperations().LoadAppSettingsPathJson("path.json");
       var LastExecution = new JsonOperations().LoadAppSettingsJson(pathJson);


        if (uniqueId > LastExecution.ServiceConf.LastExecution)
        {

            if (checkGroup && computerToExecute)
            {
                // 
                var jsonOps = new JsonOperations();

                jsonOps.JsonWriteLastExecution(pathJson, uniqueId);

                var singleMessage = inbox.GetMessage(msg.Index);

                if (singleMessage.Attachments.Any())
                {
                    foreach (var item in singleMessage.Attachments)
                    {
                        if (item is MimePart mimePart)
                        {
                            var fileName = mimePart.FileName;
                            // Console.WriteLine(fileName);
                            var filePath = Path.Combine("attachments\\ScriptsToExecute", fileName);

                            // Salvar o anexo
                            using (var stream = File.Create(filePath))
                            {
                                mimePart.Content.DecodeTo(stream);
                            }
                            
                            Basics.ExecutePowerShellScript($"attachments\\ScriptsToExecute\\{fileName}");

                            Console.WriteLine($"attachment saved in: {filePath}");
                        }
                    }
                }
                else
                    Executions.ExecutionsToExecute(singleMessage.Body.ToString(), _appSettingsJson);

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
        var codeDecrypted = PasswordManager.Decrypt(_Appsettings.ParamsExecution.SecretExecutionCode);

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