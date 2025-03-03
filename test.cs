using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public static class ScriptUtility
{
    public static void ScriptModifyAddLogEmailReturn(string path)
    {
        try
        {
            string[] existingContent = ReadExistingContent(path);
            string scriptContent = BuildScriptContent(existingContent);

            WriteScriptContent(path, scriptContent);
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }

    private static string[] ReadExistingContent(string path)
    {
        return File.Exists(path) ? File.ReadAllLines(path) : Array.Empty<string>();
    }

    private static string BuildScriptContent(string[] existingContent)
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
        scriptBuilder.AppendLine($"Start-Process -FilePath \"{emailSender}\" -ArgumentList \"subject\", \"body\", $logpath\"");

        return scriptBuilder.ToString();
    }

    private static void WriteScriptContent(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    private static void LogError(Exception ex)
    {
        Console.WriteLine(ex.Message);
        // Aqui você pode adicionar um código adicional para logar o erro em um sistema de logs ou enviar uma notificação por e-mail.
    }
}
