using CarRental.Core.Interfaces;
using CarRental.Infrastructure.Email;

using Microsoft.Extensions.Logging;

using Moq;

namespace CarRental.Tests.Integration.Emails;

public class FakeEmailSenderTests
{
    [Fact]
    public async Task should_log_info_when_email_is_sent()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FakeEmailSender>>();
        IEmailService emailSender = new FakeEmailSender(loggerMock.Object);

        // Act
        await emailSender.SendEmailAsync("to@example.com", "from@example.com", "Subject", "Body");

        // Assert
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Not actually sending an email to to@example.com")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }
}
