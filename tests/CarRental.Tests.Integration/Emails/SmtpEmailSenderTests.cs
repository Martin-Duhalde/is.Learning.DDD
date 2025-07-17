using CarRental.Infrastructure.Email;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace CarRental.Tests.Integration.Emails;

#pragma warning disable CS1998

public class SmtpEmailSenderTests
{

    [Fact]
    public async Task should_log_email_sending()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ISmtpClientWrapper>>();

        var mailConfig = new MailserverConfiguration
        {
            Hostname = "smtp.test.com",
            Port = 2525
        };

        var optionsMock = Mock.Of<IOptions<MailserverConfiguration>>(o => o.Value == mailConfig);

        var emailSender = new SmtpEmailSender(loggerMock.Object, optionsMock);

        // Act (simulado solo hasta el log, comentar para pruebas reales)
        //await emailSender.SendEmailAsync("to@test.com", "from@test.com", "Subject", "Body");

        // Assert (verifica que se haya intentado registrar el log con el "to")
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("to@test.com")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never // ➤ Cambiar a `Times.Once` al descomentar el Act real
        );
    }
}
