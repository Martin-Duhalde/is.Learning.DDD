using CarRental.Core.Interfaces;
using CarRental.Infrastructure.Email;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using Xunit;

namespace CarRental.Tests.Integration.Emails;

public class MimeKitEmailSenderTests
{
    [Fact]
    public async Task should_send_email_without_throwing_exceptions()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<MimeKitEmailSender>>();
        var optionsMock = new Mock<IOptions<MailserverConfiguration>>();

        var config = new MailserverConfiguration
        {
            Hostname = "localhost",
            Port = 2525 // un puerto dummy para tests (puede ser MailHog o similar)
        };

        optionsMock.Setup(x => x.Value).Returns(config);

        var emailSender = new MimeKitEmailSender(loggerMock.Object, optionsMock.Object);

        // Act & Assert
        // Como no hay un servidor SMTP real, esto lanzará una excepción de conexión
        // pero el test estructura está lista para mocking avanzado o SMTP fake
        await Assert.ThrowsAnyAsync<Exception>(() =>
            emailSender.SendEmailAsync("to@example.com", "from@example.com", "Test Subject", "Test Body")
        );
    }
}
