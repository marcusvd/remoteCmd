public class ReturnsExeciutions
{

    public static void ReturnsEmails(string action, string typeReturn,string returnOutput, AppSettings _appSettings)
    {
        Sender.SendEmail(_appSettings.ServerSmtp.UserName, $" {action ?? ""} - {Environment.MachineName} - {DateTime.Now}", $"{typeReturn}: {returnOutput}", "", _appSettings);
    }
}