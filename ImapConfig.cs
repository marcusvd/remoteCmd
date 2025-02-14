
    public class ImapSettings
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; }= string.Empty;
        public string Password { get; set; } = string.Empty;
        public int DelayCheckNewMail { get; set; } = 1000;
      
    }