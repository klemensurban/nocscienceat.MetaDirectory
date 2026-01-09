namespace nocscienceat.MetaDirectory.Logging.Models
{
    public class EmailLogSettings
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string SmtpHost { get; set; } = string.Empty;
        public int Port { get; set; } = 25;
        public string SecureSocketOption { get; set; } = nameof(MailKit.Security.SecureSocketOptions.None);
        public string WindowStart { get; set; } = "08:15:00";
        public string WindowEnd { get; set; } = "08:45:00";
    }
}
