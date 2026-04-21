namespace SafeBite_Backend_H6.API.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var host = _config["Email:Host"];
        var port = int.Parse(_config["Email:Port"]!);
        var username = _config["Email:Username"];
        var password = _config["Email:Password"];
        var from = _config["Email:From"];
        var enableSsl = bool.Parse(_config["Email:EnableSsl"]!);

        var smtpClient = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        var message = new MailMessage
        {
            From = new MailAddress(from!),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        message.To.Add(email);

        await smtpClient.SendMailAsync(message);
    }
}

