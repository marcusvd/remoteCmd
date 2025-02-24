using Microsoft.Win32;

public class RegistryGetSet
{

    public static string LogTxtFile { get; set; } = Path.Combine(BasePath.Path + "\\LogRegistry", "EventRegistry.txt");
    public static string LogFolderPath { get; set; } = Path.Combine(BasePath.Path, "LogRegistry");


    public static void CreateRegistryEntry(RegistryKey root, string key, string name, string value)
    {
        try
        {
            var registryKey = registryKeyCheck(root);
            var subKey = registryKey.CreateSubKey(key);

            if (registryKey != null)
            {
                subKey.SetValue(name, value);
                subKey.Close();
            }
        }
        catch (Exception ex)
        {
            if (FolderPathManager.isFolderExists(LogFolderPath))
                TextFile.Write(LogTxtFile, $"Created registry: {key}, {name}, {value}", true);
            else
            {
                FolderPathManager.CreateFolder(LogFolderPath);
                TextFile.Write(LogTxtFile, $"Error when creating registry: {key}, {name}, {value} - {ex.Message}", true);
            }


        }
    }

    public static bool isKeyNameValueExists(RegistryKey root, string key, string name)
    {

        var registryKey = registryKeyCheck(root);
        var subKey = registryKey.OpenSubKey(key);
        if (subKey != null)
        {
            object value = registryKey.GetValue(name);

            subKey.Close();

            return value != null;
        }

        return false;
    }

    public static string GetRegistryValue(RegistryKey root, string key, string name)
    {
        var registryKey = registryKeyCheck(root);
        var subKey = registryKey.OpenSubKey(key);
        if (subKey != null)
        {
            object value = subKey.GetValue(name);
            subKey.Close();
            if (value != null)
                return value.ToString();
            else
                return String.Empty;
        }

        return String.Empty;
    }
    private static RegistryKey registryKeyCheck(RegistryKey root)
    {
        if (root == Registry.LocalMachine)
            return Registry.LocalMachine;

        if (root == Registry.LocalMachine)
            return Registry.CurrentUser;

        return root;
    }

}