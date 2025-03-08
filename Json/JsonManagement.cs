using System.Text.Json;
using Microsoft.Win32;
using remoteCmd.Tasks.RegistryOperations;

public static class JsonManagement
{
    public static string jsonPath = RegistryManagement.GetRegistryValue(Registry.LocalMachine, "SOFTWARE\\RemoteCmd", "AppSettingsJson");

    public static AppSettings LoadJson(string pathJsonFile)
    {
        try
        {
            string jsonfile = File.ReadAllText(pathJsonFile);

            var jsonFile = new AppSettings();

            jsonFile = JsonSerializer.Deserialize<AppSettings>(jsonfile);

            if (jsonFile != null)
            {
                // EventLog.WriteEntry("MyService", appSettings.ServerSmtp.UserName, EventLogEntryType.Information);
                return jsonFile;
            }

        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error when load file JSON: {ex.Message}", "Error");
            // // EventLog.WriteEntry("MyService", ex.ToString(), EventLogEntryType.Error);
        }

        return new AppSettings();
    }
    public static void JsonWrite(string nameFileToSave, AppSettings json)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var objToSave = JsonSerializer.Serialize(json, options);

            File.WriteAllText(nameFileToSave, objToSave);

        }
        catch (IOException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}

