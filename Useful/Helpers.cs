
public static class BasePath
{
    public static string Path { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
}
public static class TextFile
{
    public static void Write(string path, string text, bool append = false)
    {
        try
        {
            using (StreamWriter streamWriter = new StreamWriter(path, append))
            {
                streamWriter.Write(text);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}

public static class FolderPathManager
{
    public static bool isFolderExists(string path)
    {

        if (Directory.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string CreateFolder(string path)
    {
        if (!isFolderExists(path))
        {
            var created = Directory.CreateDirectory(path);
            return created.FullName;
        }

        return path;

    }

}


public static class OsChecker
{

    public static bool IsWindows()
    {
        if (OperatingSystem.IsWindows())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}