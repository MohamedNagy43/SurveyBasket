using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using SurveyBasket.Api.Settings;

namespace SurveyBasket.Api.Services;

public class EmailService(IOptions<MailSettings> mailSettings,ILogger<EmailService> logger) : IEmailSender
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // message has (sender,receiver,subject,html bodyBuilder)
        var message = new MimeMessage()
        {
            Sender = MailboxAddress.Parse(_mailSettings.Mail),
            Subject = subject
        };
        message.To.Add(MailboxAddress.Parse(email));

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = bodyBuilder.ToMessageBody();

        _logger.LogInformation("sending email to {email}", email);

        // Connection >> Smpt here from MailKit package
        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_mailSettings.Host,_mailSettings.Port,SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
        await smtpClient.SendAsync(message);
        await smtpClient.DisconnectAsync(true);
    }
}
