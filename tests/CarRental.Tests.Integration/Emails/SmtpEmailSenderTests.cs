//using CarRental.Core.Interfaces;
//using CarRental.Infrastructure.Email;

//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//using Moq;

//namespace CarRental.Tests.Emails;

//public class SmtpEmailSenderTests
//{
  
//    [Fact]
//    public async Task SendEmailAsync_ShouldBuildMailMessageCorrectly()
//    {
//        // Arrange
//        var loggerMock = new Mock<ILogger<SmtpEmailSender>>();
//        var config = Options.Create(new MailserverConfiguration { Hostname = "smtp.example.com", Port = 2525 });
//        var emailSender = new SmtpEmailSender(loggerMock.Object, config);

//        // Act & Assert
//        // Solo validamos que no lanza excepciones con la config falsa.
//        var exception = await Record.ExceptionAsync(() =>
//            emailSender.SendEmailAsync("to@example.com", "from@example.com", "Test Subject", "Test Body"));

//        Assert.Null(exception); // No se lanza excepción
//    }
//}
