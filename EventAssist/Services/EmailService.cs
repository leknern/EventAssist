using EventAssist.Services.Interfaces;
using Serilog;
using MimeKit;
using MailKit.Net.Smtp;

namespace EventAssist.Services
{
    public class EmailService(
        IConfiguration configuration) : IEmailService
    {
        public async Task SendForgotPasswordAsync(string email, string token)
        {
            string subject = "Jelszó visszaállítása";
            string html = $"<p>Kedves felhasználó!</p>" +
                          $"<p>Kattints a következő linkre a jelszavad visszaállításához:</p>" +
                          $"<p><a href='http://localhost:9000/#/auth/reset-password/{token}'>Jelszó visszaállítása</a></p>" +
                          $"<p>Ha nem te kérted a jelszó visszaállítást, akkor ezt az e-mailt figyelmen kívül hagyhatod.</p>" +
                          $"<p>Üdvözlettel,<br/>EventAssist csapata</p>";
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("no-reply@eventassist.com"));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            { Text = html };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(configuration["SMTP:Username"], configuration["SMTP:Password"]);
            smtp.Send(message);
            smtp.Disconnect(true);

            Log.Information("Email sent successfully to {Email}", email);
        }
    }
}
