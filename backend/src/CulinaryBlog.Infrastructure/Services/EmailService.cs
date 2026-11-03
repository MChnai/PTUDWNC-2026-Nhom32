using System.Net;
using System.Net.Mail;
using CulinaryBlog.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace CulinaryBlog.Infrastructure.Services;
public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendWelcomeEmailAsync(string toEmail, string username, CancellationToken ct = default)
    {
        var host = config["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host)) throw new InvalidOperationException("Chưa cấu hình SMTP để gửi email chào mừng.");
        using var smtp = new SmtpClient(host, config.GetValue("Smtp:Port", 1025)) { EnableSsl = config.GetValue("Smtp:EnableSsl", false) };
        if (!string.IsNullOrWhiteSpace(config["Smtp:Username"])) smtp.Credentials = new NetworkCredential(config["Smtp:Username"], config["Smtp:Password"]);
        var name = WebUtility.HtmlEncode(username);
        var client = WebUtility.HtmlEncode(config["ClientUrl"] ?? "http://localhost:3000");
        using var message = new MailMessage(config["Smtp:From"] ?? "hello@culinary.local", toEmail)
        {
            Subject = "Chào mừng bạn đến với căn bếp Culinary!", IsBodyHtml = true,
            Body = $"<h1>Chào {name}!</h1><p>Căn bếp Culinary luôn chào đón bạn.</p><p><a href=\"{client}/recipes\">Khám phá công thức đầu tiên</a></p>"
        };
        await smtp.SendMailAsync(message, ct);
        logger.LogInformation("Welcome email sent successfully.");
    }
}
