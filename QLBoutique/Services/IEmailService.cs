namespace QLBoutique.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmail(string toEmail, string toName, string confirmationLink);
    }
}
