using System.Diagnostics;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService
{

    private readonly ILogger<Worker> _logger;
    private readonly ImapSettings _imapSettings;

    public Worker(ILogger<Worker> logger, IConfiguration consiguration)
    {
        _logger = logger;
        _imapSettings = consiguration.GetSection("ImapSettings").Get<ImapSettings>() ?? new ImapSettings();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Checking new messages at: {time}", DateTime.Now);

            try
            {
                using (var client = new ImapClient())
                {
                    //ignore validation certificate SSL
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // Connect to the server
                    await client.ConnectAsync(_imapSettings.Server, _imapSettings.Port, _imapSettings.UseSsl, stoppingToken);
                    _logger.LogInformation("Connected to {server} on port {port}", _imapSettings.Server, _imapSettings.Port);

                    // Authenticates on server
                    await client.AuthenticateAsync(_imapSettings.UserName, _imapSettings.Password, stoppingToken);
                    _logger.LogInformation("Authenticated on {server} successfully!", _imapSettings.Server);

                    //open inbox
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                    var messages = await inbox.FetchAsync(0, inbox.Count - 1, MessageSummaryItems.Flags | MessageSummaryItems.UniqueId, stoppingToken);


                    var fullMessage = inbox.GetMessage(inbox.Count - 1);

                    Console.WriteLine($"De: {fullMessage.From}");
                    Console.WriteLine($"Assunto: {fullMessage.Subject}");
                    Console.WriteLine($"Data: {fullMessage.Date}");
                    Console.WriteLine($"Corpo: {fullMessage.TextBody?.Trim() ?? "(Sem conteúdo de texto)"}");



                    if (messages.Last().Flags == MessageFlags.Seen)
                    {


                    }
                    else
                    {
                        inbox.AddFlags(messages.Last().UniqueId, MessageFlags.Seen, true);
                        inbox.Expunge(); //force synchronization with server
                        Process.Start(@"c:\windows\system32\calc.exe");
                        Console.WriteLine("Status: Não lida");
                    }

                    //disconnect from the server
                    await client.DisconnectAsync(true, stoppingToken);
                    _logger.LogInformation("Disconnected from {server} successfully!", _imapSettings.Server);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking new messages");
            }

            await Task.Delay(_imapSettings.DelayCheckNewMail, stoppingToken);
        }
    }
}