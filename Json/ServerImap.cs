public class ServerImap
{
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordImap { get; set; } = string.Empty;
}