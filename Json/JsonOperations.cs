using System.Text.Json;



public class JsonOperations
{
    private RemoteCmdJsonConf? _remoteCmdJsonConf;
    public JsonOperations()
    {
        _remoteCmdJsonConf = null;
    }
    public JsonOperations(
        RemoteCmdJsonConf RemoteCmdJsonConf
        )
    {
        _remoteCmdJsonConf = RemoteCmdJsonConf;
    }



    public void JsonBuilder()
    {

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(_remoteCmdJsonConf, options);
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

    public RemoteCmdJsonConf jsonLoad(string pathJsonFile)
    {
        try
        {
            string jsonfile = File.ReadAllText(pathJsonFile);
            var appSettings = new RemoteCmdJsonConf();
            appSettings = JsonSerializer.Deserialize<RemoteCmdJsonConf>(jsonfile);

            if (appSettings != null)
                return appSettings;

        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error when load file JSON: {ex.Message}", "Error");
        }

        return new RemoteCmdJsonConf();
    }

    public void FillForm(RemoteCmdJsonConf entity)
    {

    }




}