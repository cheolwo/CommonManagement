using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using System.Configuration;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string htmlMessage);
}

public class EmailSender : IEmailSender
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _fromAddress;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;

    public EmailSender(IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection("EmailSettings");

        _smtpHost = emailSettings["SmtpHost"];
        _smtpPort = int.Parse(emailSettings["SmtpPort"]);
        _fromAddress = emailSettings["SmtpFromAddress"];
        _smtpUsername = emailSettings["SmtpUsername"];
        _smtpPassword = emailSettings["SmtpPassword"];
    }

    public async Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_fromAddress));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = htmlMessage };

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_smtpHost, _smtpPort, true);
        await smtpClient.AuthenticateAsync(_smtpUsername, _smtpPassword);
        await smtpClient.SendAsync(email);
        await smtpClient.DisconnectAsync(true);
    }
}
