using System.Net;
using System.Net.Mail;

public static class EmailSender
{
    public static void SendEmail(string to, string subject, string body, string attachmentPath)
    {
        var jsonOps = new JsonOperations();

        var _pathJson = jsonOps.LoadAppSettingsPathJson("path.json");

        var _appSettingsJson = new JsonOperations().LoadAppSettingsJson(_pathJson.Path);

        var message = new MailMessage(_appSettingsJson.ServerSmtp.UserName, to, subject, body);

        // Adicionar anexo ao email
        if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
        {
            Attachment attachment = new Attachment(attachmentPath);
            message.Attachments.Add(attachment);
        }

        SmtpClient SmtpClient = new SmtpClient("smtp.nostopti.com.br")
        {
            Port = _appSettingsJson.ServerSmtp.Port,
            Credentials = new NetworkCredential(_appSettingsJson.ServerSmtp.UserName, PasswordManager.Decrypt(_appSettingsJson.ServerSmtp.Password)),
        };
        SmtpClient.SendCompleted += (s, e) =>
        {
            SmtpClient.Dispose();
            message.Dispose();
        };
        try
        {
            SmtpClient.SendAsync(message, null);
        }
        catch (SmtpFailedRecipientException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }



}