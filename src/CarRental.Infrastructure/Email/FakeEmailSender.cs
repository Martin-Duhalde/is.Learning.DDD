/// MIT License © 2025 Martín Duhalde + ChatGPT
/// Code From: Clean.Architecture.Infrastructure (Ardalis)

using CarRental.Core.Interfaces;

namespace CarRental.Infrastructure.Email;

public class FakeEmailSender(ILogger<FakeEmailSender> logger) : IEmailService
{
    private readonly ILogger<FakeEmailSender> _logger = logger;

    public Task SendEmailAsync(string to, string from, string subject, string body)
    {
        _logger.LogInformation("Not actually sending an email to {to} from {from} with subject {subject}", to, from, subject);
        return Task.CompletedTask;
    }
}
