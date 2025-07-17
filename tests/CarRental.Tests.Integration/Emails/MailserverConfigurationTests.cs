using CarRental.Infrastructure.Email;

namespace CarRental.Tests.Integration.Emails;
public class MailserverConfigurationTests
{
    [Fact]
    public void should_have_default_values()
    {
        var config = new MailserverConfiguration();

        Assert.Equal("localhost", config.Hostname);
        Assert.Equal(25, config.Port);
    }

    [Fact]
    public void should_allow_setting_custom_values()
    {
        var config = new MailserverConfiguration
        {
            Hostname = "smtp.example.com",
            Port = 587
        };

        Assert.Equal("smtp.example.com", config.Hostname);
        Assert.Equal(587, config.Port);
    }
}