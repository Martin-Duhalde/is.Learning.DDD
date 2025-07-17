using System.Net.Mail;

namespace CarRental.Infrastructure.Email
{
    public interface ISmtpClientWrapper
    {
        Task SendMailAsync(MailMessage message);
    }
}
