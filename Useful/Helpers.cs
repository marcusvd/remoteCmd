public static class TextFileWriter
{
    public static void Write(string path, string text)
    {
        try
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
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