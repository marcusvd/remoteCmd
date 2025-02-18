using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using MailKit;
using MimeKit;
using Org.BouncyCastle.Crypto.Prng;

public class Conditions
{
    private readonly RemoteCmdJsonConf _remoteCmdJsonConf;
    public Conditions(RemoteCmdJsonConf remoteCmdJsonConf)
    {
        _remoteCmdJsonConf = remoteCmdJsonConf;
    }

    public void ConditionsToExecute(MimeMessage lastMsg, UniqueId UniqueId)
    {
        var codeExecution = GetCodeExecution(lastMsg.Subject);

        var groups = GetGroupsExecution(lastMsg.Subject);

        var computers = GetCumpotersExecution(lastMsg.Subject);


        string groupToExecution = String.Empty;

        foreach (var item in groups)
        {
            if (item == _remoteCmdJsonConf.ParamsExecution.GroupExecution)
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

        var checkedCodeExecution = CheckCodeExecution(codeExecution);
        var checkGroup = !String.IsNullOrEmpty(groupToExecution);
        var computerToExecute = computerFound;

        // Console.WriteLine($"RESULT: {checkedCodeExecution}, {checkGroup}, {computerToExecute}");

        if (checkedCodeExecution && checkGroup && computerToExecute)
        {
            Console.WriteLine(lastMsg.Body.ToString().ToLower().Contains("desligar"));
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
        var codeDecrypted = PasswordManager.Decrypt(_remoteCmdJsonConf.ParamsExecution.SecretExecutionCode);

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
        catch (Exception ex)
        {
            return false;
        }


        return false;
    }


}