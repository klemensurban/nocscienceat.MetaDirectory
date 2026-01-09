using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using nocscienceat.MetaDirectory.Logging.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Logging
{
    public static class MailLog
    {
        public static async Task Send(IConfiguration? configuration, StringBuilder logInfo)
        {
            if (configuration is null)
                return;

            EmailLogSettings emailSettings = configuration?.GetSection("EmailLog").Get<EmailLogSettings>() ?? new EmailLogSettings();
            
            if (!TimeSpan.TryParse(emailSettings.WindowStart, out TimeSpan startTime))
            {
                startTime = new TimeSpan(8, 15, 0);
            }

            if (!TimeSpan.TryParse(emailSettings.WindowEnd, out TimeSpan endTime))
            {
                endTime = new TimeSpan(8, 45, 0);
            }

            TimeSpan currentTime = DateTime.UtcNow.TimeOfDay;
            if (currentTime < startTime || currentTime > endTime)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(emailSettings.From) ||
                string.IsNullOrWhiteSpace(emailSettings.To) ||
                string.IsNullOrWhiteSpace(emailSettings.SmtpHost))
            {
                return;
            }

            if (!Enum.TryParse(emailSettings.SecureSocketOption, out SecureSocketOptions secureSocketOption))
            {
                secureSocketOption = SecureSocketOptions.None;
            }

            MimeMessage message = new();
            message.From.Add(MailboxAddress.Parse(emailSettings.From));
            message.To.Add(MailboxAddress.Parse(emailSettings.To));
            message.Subject = string.IsNullOrWhiteSpace(emailSettings.Subject) ? "Logs from nocscienceat.MetaDirectory" : emailSettings.Subject;
            message.Body = new TextPart("plain")
            {
                Text = logInfo.ToString()
            };

            using SmtpClient client = new();
            if (secureSocketOption is SecureSocketOptions.StartTls or SecureSocketOptions.SslOnConnect or SecureSocketOptions.StartTlsWhenAvailable)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(emailSettings.SmtpHost, emailSettings.Port, secureSocketOption);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
