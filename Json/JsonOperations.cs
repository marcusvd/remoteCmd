using System.Diagnostics;
using System.Text.Json;



public class JsonOperations
{
    private AppSettings? _AppSettings;
    public JsonOperations()
    {

    }
    public JsonOperations(
        AppSettings AppSettings
        )
    {
        _AppSettings = AppSettings;
    }



    public void JsonBuilder()
    {

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(_AppSettings, options);
        string nameFileToSave = "appSettings.json";

        try
        {
            File.WriteAllText(nameFileToSave, json);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine(json);
    }

    public AppSettings LoadAppSettingsJson(string pathJsonFile)
    {
        try
        {
            string jsonfile = File.ReadAllText(pathJsonFile);
            var appSettings = new AppSettings();
            appSettings = JsonSerializer.Deserialize<AppSettings>(jsonfile);

            if (appSettings != null)
            {
                EventLog.WriteEntry("MyService", appSettings.ServerSmtp.UserName, EventLogEntryType.Information);
                return appSettings;
            }

        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error when load file JSON: {ex.Message}", "Error");
            EventLog.WriteEntry("MyService", ex.ToString(), EventLogEntryType.Error);
        }

        return new AppSettings();
    }

    public AppSettingsPath LoadAppSettingsPathJson(string pathJsonFile)
    {
        try
        {
            string jsonfile = File.ReadAllText(pathJsonFile);
            var appSettingsPath = new AppSettingsPath();
            appSettingsPath = JsonSerializer.Deserialize<AppSettingsPath>(jsonfile);

            if (appSettingsPath != null)
                return appSettingsPath;

        }

        catch (Exception)
        {
            Console.WriteLine($"JSON file not found.", "Information");
        }

        return new AppSettingsPath();
    }

    public void JsonWriteLastExecution(string pathJsonFile, int uniqueId)
    {
        string nameFileToSave = pathJsonFile;
        string json;

        if (File.Exists(nameFileToSave))
        {
            json = File.ReadAllText(nameFileToSave);
        }
        else
        {
            Console.WriteLine("Arquivo JSON não encontrado.");
            return;
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        try
        {
            // Desserializar JSON existente
            var appSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();

            // Atualizar propriedade desejada
            appSettings.ServiceConf.LastExecution = uniqueId;

            // Serializar novamente o objeto para JSON
            json = JsonSerializer.Serialize(appSettings, options);

            // Salvar JSON atualizado no arquivo
            File.WriteAllText(nameFileToSave, json);

            Console.WriteLine("JSON updated successfully.");
            Console.WriteLine(json);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}
