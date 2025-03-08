using System.Diagnostics;
using System.Web;
using MimeKit;
using remoteCmd.Scripts;
using remoteCmd.Tasks.Basic;


public class CallToExecute
{
    private readonly AppSettings _appSettings;
    private readonly MimeMessage _singleMessage;
    public CallToExecute(AppSettings Appsettings, MimeMessage singleMessage)
    {
        _appSettings = Appsettings;
        _singleMessage = singleMessage;
    }

    public void ScriptAttachmentsToExecute()
    {
        //var pathScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\ScriptsToExecute");

        var pathScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\ScriptsToExecute");
        if (!Directory.Exists(pathScript)) Directory.CreateDirectory(pathScript);

        var path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\WindowsLogs");
        if (!Directory.Exists(path2)) Directory.CreateDirectory(path2);


        try
        {
            foreach (var item in _singleMessage.Attachments)
            {
                if (item is MimePart mimePart)
                {
                    var fileName = mimePart.FileName;

                    if (!Path.HasExtension(mimePart.FileName))
                        fileName = mimePart.FileName + ".ps1";

                    var filePath = Path.Combine(pathScript, fileName);

                    // Salvar o anexo
                    using (var stream = File.Create(filePath))
                        mimePart.Content.DecodeTo(stream);

                    if (_singleMessage.Body.ToString().Contains("AdvancedElevatedExecution"))
                    {
                        Console.WriteLine($"Advanced Elevated Execution - was received. Script {filePath} will execute in the next reboot.");
                        EventLog.WriteEntry("RemoteCmd", $"Advanced Elevated Execution - was received. Script {filePath} will execute in the next reboot.", EventLogEntryType.Information);

                        var userNamePassword = HttpUtility.HtmlDecode(_singleMessage.Body.ToString()).Split('|');

                        Advanced.GetSaveCurrentUserNameLogged();
                        Advanced.ExecuteScriptElevatedAfterLogon(filePath);
                        Advanced.ConfigureAutoLogon(filePath, _appSettings, userNamePassword[1], userNamePassword[2], userNamePassword[3]);
                        BasicsManagement.Reboot(_appSettings);
                    }

                    if (_singleMessage.Body.ToString().Contains("PowershellScriptRun"))
                        Advanced.PowershellScriptRun(filePath, _appSettings);

                    if (_singleMessage.Body.ToString().Contains("ScheduleBasicTaskPowerShellScript"))
                        Advanced.ScheduleBasicTaskPowerShellScript(filePath, _appSettings);

                    if (_singleMessage.Body.ToString().Contains("ScheduleTaskHighestPowerShellScript"))
                    {
                        var userNamePassword = HttpUtility.HtmlDecode(_singleMessage.Body.ToString()).Split('|');
                        Advanced.ScheduleTaskHighestPowerShellScript(filePath, _appSettings, userNamePassword[1], userNamePassword[2]);
                    }

                    Console.WriteLine($"attachment saved in: {filePath}");
                }
            }
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry("RemoteCmd", $"{ex.ToString()} attached .ps1 file not found.", EventLogEntryType.Error);
        }


    }


}