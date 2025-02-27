using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using PasswordManagement;

public static class EmailSender
{
    public static void SendEmail(string to, string subject, string body, string attachmentPath, AppSettings _appSettingsJson)
    {

        var message = new MailMessage(_appSettingsJson.ServerSmtp.UserName, to, subject, body);

        // Adicionar anexo ao email
        if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
        {
            Attachment attachment = new Attachment(attachmentPath);
            message.Attachments.Add(attachment);
        }

        SmtpClient SmtpClient = new SmtpClient(_appSettingsJson.ServerSmtp.Server)
        {
            Port = _appSettingsJson.ServerSmtp.Port,
            Credentials = new NetworkCredential(_appSettingsJson.ServerSmtp.UserName, PasswordManager.Decrypt(_appSettingsJson.ServerSmtp.Password)),
            EnableSsl = _appSettingsJson.ServerSmtp.UseSsl
        };
        SmtpClient.SendCompleted += (s, e) =>
        {
            SmtpClient.Dispose();
            message.Dispose();
        };

        SmtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

        try
        {
            SmtpClient.SendAsync(message, null);
        }
        catch (SmtpFailedRecipientException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        // Verificar se houve um erro
        if (e.Error != null)
        {
            Console.WriteLine($"Erro ao enviar email: {e.Error.Message}");
        }
        else if (e.Cancelled)
        {
            Console.WriteLine("Envio de email cancelado.");
        }
        else
        {
            Console.WriteLine("Email enviado com sucesso!");
        }

        // Limpar o objeto SmtpClient e MailMessage
        ((SmtpClient)sender).Dispose();
        MailMessage mail = e.UserState as MailMessage;
        mail?.Dispose();
    }


}