namespace SafeBite_Backend_H6.API.Services;

public class EmailSender : IEmailSender<ApplicationUser>
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var html = $"""
            <h1>Confirm your email</h1>
            <p>
                Welcome to SafeBite.
            </p>
            <p>
                Click the link below to confirm your account:
            </p>
            <p>
                <a href="{confirmationLink}">
                    Confirm Email
                </a>
            </p>
            """;

        await SendEmailAsync(email, "Confirm your SafeBite account", html);
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

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {

        var html = $"""
        <h1>Password Reset</h1>
        <p>
            Use the following password reset code:
        </p>
        <h2 style="user-select:all;-webkit-user-select:all;" >{resetCode}</h2>
        <p>
            If you did not request a password reset, you can ignore this email.
        </p>
        """;

        await SendEmailAsync(email, "SafeBite password reset", html);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var html = $"""
            <h1>Password Reset</h1>
            <p>
                Click the link below to reset your password:
            </p>
            <p>
                <a href="{resetLink}">
                    Reset Password
                </a>
            </p>
            """;

        await SendEmailAsync(email, "SafeBite password reset", html);
    }
}

