using MailKit;
using MailKit.Net.Imap;
// using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Connect : BackgroundService
{

    private readonly ILogger<Connect> _logger;
    // private readonly RemoteCmdJsonConf _remoteCmdJsonConf;

    // public Worker(ILogger<Worker> logger, IConfiguration consiguration)
    public Connect(ILogger<Connect> logger)
    {
        _logger = logger;
        // _remoteCmdJsonConf = consiguration.GetSection("RemoteCmdJsonConf").Get<RemoteCmdJsonConf>() ?? new RemoteCmdJsonConf();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // var jsonOps = new JsonOperations();

        var _remoteCmdJsonConf = new JsonOperations().jsonLoad("appSettings.json");

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
                    await client.ConnectAsync(_remoteCmdJsonConf.ServerImap.Server, _remoteCmdJsonConf.ServerImap.Port, _remoteCmdJsonConf.ServerImap.UseSsl, stoppingToken);
                    _logger.LogInformation("Connected to {server} on port {port}", _remoteCmdJsonConf.ServerImap.Server, _remoteCmdJsonConf.ServerImap.Port);

                    // Authenticates on server
                    await client.AuthenticateAsync(_remoteCmdJsonConf.ServerImap.UserName, PasswordManager.Decrypt(_remoteCmdJsonConf.ServerImap.PasswordImap), stoppingToken);
                    _logger.LogInformation("Authenticated on {server} successfully!", _remoteCmdJsonConf.ServerImap.Server);

                    //open inbox
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                    //check new messages
                    if (inbox.Count > 0)
                    {
                        var messages = await inbox.FetchAsync(0, inbox.Count - 1, MessageSummaryItems.Flags | MessageSummaryItems.UniqueId, stoppingToken);


                        var fullMessage = inbox.GetMessage(messages.Last().UniqueId);
                        // Console.WriteLine(fullMessage);

                        var lastMsg = messages.Last();

                        Conditions Cond = new Conditions(_remoteCmdJsonConf);

                        Cond.ConditionsToExecute(fullMessage, messages.Last().UniqueId);



                        // if (messages.Last().Flags == MessageFlags.Seen)
                        // {


                        // }
                        // else
                        // {
                        //     inbox.AddFlags(messages.Last().UniqueId, MessageFlags.Seen, true);
                        //     inbox.Expunge(); //force synchronization with server
                        //                      //  Process.Start(@"c:\windows\system32\calc.exe");
                        //     Console.WriteLine("Status: Não lida");
                        // }
                    }
                    else
                    {
                        Console.WriteLine("Inbox is empty.");
                    }

                    //disconnect from the server
                    await client.DisconnectAsync(true, stoppingToken);
                    _logger.LogInformation("Disconnected from {server} successfully!", _remoteCmdJsonConf.ServerImap.Server);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking new messages");
            }

            await Task.Delay(_remoteCmdJsonConf.ServiceConf.DelayCheckNewMail, stoppingToken);
        }
    }
}