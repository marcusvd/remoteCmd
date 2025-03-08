using Microsoft.Win32;
namespace remoteCmd.Tasks.RegistryOperations;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Windows-only API")]
public class RegistryManagement
{
    public static void CreateRegistryEntry(RegistryKey root, string key, string name, dynamic value)
    {
        var registryKey = registryKeyCheck(root);
        var subKey = registryKey.CreateSubKey(key);

        if (registryKey != null)
        {
            subKey.SetValue(name, value);
            subKey.Close();
        }
    }

    public static bool isKeyNameValueExists(RegistryKey root, string key, string name)
    {
        var registryKey = registryKeyCheck(root);
        var subKey = registryKey.OpenSubKey(key);
        if (subKey != null)
        {
            object? value = registryKey?.GetValue(name);

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
            object? value = subKey?.GetValue(name);
            subKey?.Close();
            if (value != null)
                return value?.ToString() ?? String.Empty;
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