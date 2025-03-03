
using System.Text;

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

    public static void ScriptModify(string path, Func<string[], string> returnScriptMounted)
    {
        try
        {
            string[] existingContent = ReadExistingContent(path);
            WriteScriptContent(path, returnScriptMounted(existingContent));
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }

    private static string[] ReadExistingContent(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"O arquivo '{path}' não foi encontrado.");
        }
        return File.ReadAllLines(path);
    }

    public static string LogResultActionReturnEmail(string[] existingContent)
    {
        string emailSender = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailSender.exe");

        var scriptBuilder = new StringBuilder();
        scriptBuilder.AppendLine("$logpath = $env:SystemDrive + \"\\windows\\temp\\log.txt\"");
        scriptBuilder.AppendLine("Start-Transcript -Path $logPath -Force");
        scriptBuilder.AppendLine("try {");

        foreach (string line in existingContent)
        {
            scriptBuilder.AppendLine(line);
        }

        scriptBuilder.AppendLine("}");
        scriptBuilder.AppendLine("catch {");
        scriptBuilder.AppendLine("    Write-Host \"An error occurred: $_\"");
        scriptBuilder.AppendLine("}");
        scriptBuilder.AppendLine("finally {");
        scriptBuilder.AppendLine("    Stop-Transcript");
        scriptBuilder.AppendLine("}");
        scriptBuilder.AppendLine($"Start-Process -FilePath \"{emailSender}\" -ArgumentList 'Action executed result of' + {Environment.MachineName}, $logpath, $logpath");


        return scriptBuilder.ToString();
    }

    public static string DisableAutoLogon(string[] existingContent)
    {

        var stringBuilder = new StringBuilder();

        foreach (string line in existingContent)
        {
            stringBuilder.AppendLine(line);
        }
        string autoAdminLogon = @"Set-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon' -Name 'AutoAdminLogon' -Value 0";

        string currentLoggedUser = @"$lastLogon = Get-ItemPropertyValue -Path 'HKLM:\SOFTWARE\RemoteCmd' -Name 'current logged user'";

        string lastLogon = @"Set-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon' -Name 'LastUsedUsername' -Value $lastLogon";

        string shutdown = @"shutdown -r -f -t 40";

        stringBuilder.AppendLine(autoAdminLogon);
        stringBuilder.AppendLine(currentLoggedUser);
        stringBuilder.AppendLine(lastLogon);
        stringBuilder.AppendLine(shutdown);

        return stringBuilder.ToString();

    }

    private static void WriteScriptContent(string path, string content)
    {
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(content);
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    private static void LogError(Exception ex)
    {
        Console.WriteLine(ex.Message);
        // Aqui você pode adicionar um código adicional para logar o erro em um sistema de logs ou enviar uma notificação por e-mail.
    }

    // public static string LogResultActionReturnEmail(string filePath)
    // {
    //     string emailSender = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailSender.exe");

    //     var stringBuilder = new StringBuilder();
    //     stringBuilder.AppendLine("$logpath = $env:SystemDrive + \"\\windows\\temp\\log.txt\"");
    //     stringBuilder.AppendLine("Start-Transcript -Path $logPath -Force");
    //     stringBuilder.AppendLine("try {");

    //     foreach (string line in ReadExistingContent(filePath))
    //     {
    //         stringBuilder.AppendLine(line);
    //     }

    //     stringBuilder.AppendLine("}");
    //     stringBuilder.AppendLine("catch {");
    //     stringBuilder.AppendLine("    Write-Host \"An error occurred: $_\"");
    //     stringBuilder.AppendLine("}");
    //     stringBuilder.AppendLine("finally {");
    //     stringBuilder.AppendLine("    Stop-Transcript");
    //     stringBuilder.AppendLine("}");
    //     stringBuilder.AppendLine($"Start-Process -FilePath \"{emailSender}\" -ArgumentList \"subject\", \"body\", $logpath\"");

    //     return stringBuilder.ToString();

    // }


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