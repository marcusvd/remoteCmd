// using System.Diagnostics;
// using Microsoft.Extensions.Logging;

// public class test
// {

//     public void TestRun(ILogger<Connect> _logger)
//     {

//         _logger.LogInformation("ExecuteAsync iniciado");

//         try
//         {
//             _logger.LogInformation("Criando instância de JsonOperations");
//             var jsonOps = new JsonOperations();

//             // Defina um caminho absoluto para o arquivo JSON
//             var pathJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "path.json");
//             _logger.LogInformation($"Carregando caminho do arquivo JSON a partir de {pathJsonPath}");
//             var _pathJson = jsonOps.LoadAppSettingsPathJson(pathJsonPath);

//             if (_pathJson != null)
//             {
//                 var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathJson.Path);
//                 _logger.LogInformation($"Caminho carregado: {appSettingsPath}");
//                 var _appSettingsJson = jsonOps.LoadAppSettingsJson(appSettingsPath);

//                 if (_appSettingsJson != null && _appSettingsJson.ServerImap != null)
//                 {
//                     _logger.LogInformation($"Usuário do IMAP: {_appSettingsJson.ServerImap.UserName}");

//                     EventLog.WriteEntry("MyService", _appSettingsJson.ServerImap.UserName, EventLogEntryType.Error);
//                     Console.WriteLine(_appSettingsJson.ServerImap.UserName);
//                 }
//                 else
//                 {
//                     _logger.LogWarning("Configurações do JSON carregadas são nulas ou incompletas.");
//                 }
//             }
//             else
//             {
//                 _logger.LogWarning("Falha ao carregar o caminho do JSON.");
//             }
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Erro durante a execução do ExecuteAsync");
//             EventLog.WriteEntry("MyService", ex.ToString(), EventLogEntryType.Error);
//         }
//     }



// }