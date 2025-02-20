using System.Text.Json;



public class JsonOperations
{
    private Appsettings? _Appsettings;
    public JsonOperations()
    {
        _Appsettings = null;
    }
    public JsonOperations(
        Appsettings Appsettings
        )
    {
        _Appsettings = Appsettings;
    }



    public void JsonBuilder()
    {

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(_Appsettings, options);
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

    public Appsettings LoadAppSettingsJson(string pathJsonFile)
    {
        try
        {
            string jsonfile = File.ReadAllText(pathJsonFile);
            var appSettings = new Appsettings();
            appSettings = JsonSerializer.Deserialize<Appsettings>(jsonfile);

            if (appSettings != null)
                return appSettings;

        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error when load file JSON: {ex.Message}", "Error");
        }

        return new Appsettings();
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
            var appSettings = JsonSerializer.Deserialize<Appsettings>(json) ?? new Appsettings();

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
