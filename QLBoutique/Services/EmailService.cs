using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QLBoutique.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendConfirmationEmail(string toEmail, string toName, string confirmationLink)
    {
        var fromEmail = _configuration["EmailSettings:FromEmail"];
        var password = _configuration["EmailSettings:Password"];
        var smtpHost = _configuration["EmailSettings:SmtpHost"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

        var mail = new MailMessage
        {
            From = new MailAddress(fromEmail, "QLBoutique"),
            Subject = "Xác nhận đăng ký tài khoản",
            Body = $"<p>Xin chào {toName},</p><p>Vui lòng xác nhận tài khoản bằng cách bấm vào liên kết sau:</p><p><a href='{confirmationLink}'>Xác nhận tài khoản</a></p>",
            IsBodyHtml = true
        };
        mail.To.Add(toEmail);

        var smtp = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(fromEmail, password),
            EnableSsl = true
        };

        await smtp.SendMailAsync(mail);
    }
}
