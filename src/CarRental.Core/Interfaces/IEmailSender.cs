/// MIT License © 2025 Martín Duhalde + ChatGPT
/// 
/// Interface: IEmailService
/// 📧 Core: Defines the contract for sending email notifications.
///
namespace CarRental.Core.Interfaces;


/// <summary>
/// 📧 Email service interface.
///
/// Provides a method for sending email messages asynchronously.
/// Implementations may use SMTP, third-party APIs, or mocked services for testing.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email message asynchronously.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="from">The sender's email address.</param>
    /// <param name="subject">The subject line of the email.</param>
    /// <param name="body">The HTML or plain-text body content of the email.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    Task SendEmailAsync(string to, string from, string subject, string body);
}
