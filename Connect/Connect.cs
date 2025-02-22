
using System.Diagnostics;
using System.Text.Json;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Connect : BackgroundService
{

    private readonly ILogger<Connect> _logger;
    private readonly AppSettingsPath _pathJson;


    // public Worker(ILogger<Worker> logger, IConfiguration consiguration)
    public Connect(ILogger<Connect> logger, IConfiguration configuration)
    {
        _logger = logger;
        _pathJson = configuration.Get<AppSettingsPath>() ?? new AppSettingsPath();

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jsonOps = new JsonOperations();
        var _appSettings = jsonOps.LoadAppSettingsJson(_pathJson.Path);

        // var pathJsonPath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathJson.Path);
        // var _appSettingsJson = new JsonOperations().LoadAppSettingsJson(pathJsonPath2);

        EventLog.WriteEntry("MyService", _appSettings.ServerImap.Server, EventLogEntryType.Error);
        // Console.WriteLine(_appSettingsJson.ServerImap.UserName);



        // EmailSender.SendEmail(_appSettingsJson.ServerSmtp.UserName, $"Hardware Report - {Environment.MachineName} - {DateTime.Now}", await HardwareReport.GetHardwareReportAsync(), "");
        // await HardwareReport.GetHardwareReportAsync();


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
                    await client.ConnectAsync(_appSettings.ServerImap.Server, _appSettings.ServerImap.Port, _appSettings.ServerImap.UseSsl, stoppingToken);
                    _logger.LogInformation("Connected to {server} on port {port}", _appSettings.ServerImap.Server, _appSettings.ServerImap.Port);

                    // Authenticates on server
                    await client.AuthenticateAsync(_appSettings.ServerImap.UserName, PasswordManager.Decrypt(_appSettings.ServerImap.Password), stoppingToken);
                    _logger.LogInformation("Authenticated on {server} successfully!", _appSettings.ServerImap.Server);


                    //open inbox
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                    //check new messages
                    if (inbox.Count > 0)
                    {
                        var messages = await inbox.FetchAsync(0, inbox.Count - 1, MessageSummaryItems.Body | MessageSummaryItems.All | MessageSummaryItems.Envelope | MessageSummaryItems.Flags | MessageSummaryItems.UniqueId, stoppingToken);


                        var firstFilter = messages.Where(x => x.Envelope.Subject.Contains(PasswordManager.Decrypt(_appSettings.ParamsExecution.SecretExecutionCode)));
                        if (firstFilter.Count() > 0)
                        {
                            Conditions Cond = new Conditions(_appSettings);

                            var uniqueIdToString = firstFilter.Last().UniqueId.ToString();

                            Cond.ConditionsToExecute(firstFilter.Last(), int.Parse(uniqueIdToString), inbox, _appSettings, _pathJson.Path);
                        }
                        else
                            Console.WriteLine("Inbox is empty.");

                    }
                    else
                        Console.WriteLine("Inbox is empty.");


                    //disconnect from the server
                    await client.DisconnectAsync(true, stoppingToken);
                    _logger.LogInformation("Disconnected from {server} successfully!", _appSettings.ServerImap.Server);
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
            await Task.Delay(_appSettings.ServiceConf.DelayCheckNewMail, stoppingToken);

        }
        await Task.Delay(_appSettings.ServiceConf.DelayCheckNewMail, stoppingToken);

    }
}