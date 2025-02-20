
using System.Diagnostics;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Connect : BackgroundService
{

    private readonly ILogger<Connect> _logger;
    // private readonly RemoteCmdJsonConf _appSettingsJson;

    // public Worker(ILogger<Worker> logger, IConfiguration consiguration)
    public Connect(ILogger<Connect> logger)
    {
        _logger = logger;
        // _remoteCmdJsonConf = consiguration.GetSection("RemoteCmdJsonConf").Get<RemoteCmdJsonConf>() ?? new RemoteCmdJsonConf();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jsonOps = new JsonOperations();

        var _pathJson = jsonOps.LoadAppSettingsPathJson("path.json");

        var _appSettingsJson = new JsonOperations().LoadAppSettingsJson(_pathJson.Path);


        await HardwareReport.GetHardwareReportAsync();
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
                    await client.ConnectAsync(_appSettingsJson.ServerImap.Server, _appSettingsJson.ServerImap.Port, _appSettingsJson.ServerImap.UseSsl, stoppingToken);
                    _logger.LogInformation("Connected to {server} on port {port}", _appSettingsJson.ServerImap.Server, _appSettingsJson.ServerImap.Port);

                    // Authenticates on server
                    await client.AuthenticateAsync(_appSettingsJson.ServerImap.UserName, PasswordManager.Decrypt(_appSettingsJson.ServerImap.Password), stoppingToken);
                    _logger.LogInformation("Authenticated on {server} successfully!", _appSettingsJson.ServerImap.Server);


                    //open inbox
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                    //check new messages
                    if (inbox.Count > 0)
                    {
                        var messages = await inbox.FetchAsync(0, inbox.Count - 1, MessageSummaryItems.Body | MessageSummaryItems.All | MessageSummaryItems.Envelope | MessageSummaryItems.Flags | MessageSummaryItems.UniqueId, stoppingToken);


                        var firstFilter = messages.Where(x => x.Envelope.Subject.Contains(PasswordManager.Decrypt(_appSettingsJson.ParamsExecution.SecretExecutionCode)));

                        Conditions Cond = new Conditions(_appSettingsJson);

                        var uniqueIdToString = firstFilter.Last().UniqueId.ToString();

                        Cond.ConditionsToExecute(firstFilter.Last(), int.Parse(uniqueIdToString), inbox);
                    }
                    else
                    {
                        Console.WriteLine("Inbox is empty.");
                    }

                    //disconnect from the server
                    await client.DisconnectAsync(true, stoppingToken);
                    _logger.LogInformation("Disconnected from {server} successfully!", _appSettingsJson.ServerImap.Server);
                    TextFileWriter.Write("ServiceError.txt", "");
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                _logger.LogError(ex, "Error checking new messages");
                TextFileWriter.Write("ServiceError.txt", "Error connecting to server: Incorrect port, authentication type or imap address.");
                Console.WriteLine(ex.Message);

            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                TextFileWriter.Write("ServiceError.txt", "Incorrect username or password.");
                Console.WriteLine(ex.Message);
            }
            await Task.Delay(_appSettingsJson.ServiceConf.DelayCheckNewMail, stoppingToken);
        }
    }
}