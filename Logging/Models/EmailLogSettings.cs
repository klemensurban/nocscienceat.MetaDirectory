using System;
using System.Collections.Generic;

namespace nocscienceat.MetaDirectory.Logging.Models
{
    /// <summary>
    /// Configuration settings for email log delivery with optional recipient validation.
    /// </summary>
    public class EmailLogSettings
    {
        /// <summary>
        /// Sender email address.
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// Array of recipient email addresses for log distribution.
        /// </summary>
        public List<string> To { get; set; } = new List<string>();

        /// <summary>
        /// Email subject line (defaults to generic message if not set).
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// SMTP server hostname or IP address.
        /// </summary>
        public string SmtpHost { get; set; } = string.Empty;

        /// <summary>
        /// SMTP server port (default 25).
        /// </summary>
        public int Port { get; set; } = 25;

        /// <summary>
        /// TLS/SSL security option for SMTP connection.
        /// </summary>
        public string SecureSocketOption { get; set; } = nameof(MailKit.Security.SecureSocketOptions.None);

        /// <summary>
        /// Time window start (UTC) for sending logs (HH:mm:ss format).
        /// </summary>
        public string WindowStart { get; set; } = "08:15:00";

        /// <summary>
        /// Time window end (UTC) for sending logs (HH:mm:ss format).
        /// </summary>
        public string WindowEnd { get; set; } = "08:45:00";
    }
}
