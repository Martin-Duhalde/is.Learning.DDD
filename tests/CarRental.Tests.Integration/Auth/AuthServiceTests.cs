/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Auth;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

namespace CarRental.Tests.Integration.Auth;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>>     /**/ _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>>   /**/ _signInManagerMock;
    private readonly Mock<IConfiguration>                   /**/ _configMock;
    private readonly Mock<ILogger<AuthService>>             /**/ _loggerMock;
    private readonly Mock<ICustomerRepository>              /**/ _customerRepoMock;
    private readonly AuthService                            /**/ _authService;

    public AuthServiceTests()
    {
        _userManagerMock    /**/ = MockHelpers.MockUserManager<ApplicationUser>();
        _signInManagerMock  /**/ = MockHelpers.MockSignInManager(_userManagerMock.Object);
        _configMock         /**/ = new Mock<IConfiguration>();
        _loggerMock         /**/ = new Mock<ILogger<AuthService>>();
        _customerRepoMock   /**/ = new Mock<ICustomerRepository>();

        // Setup JWT config keys (example)
        _configMock.Setup(c => c["Jwt:SecretKey"]).Returns("SuperSecretKeyParaJWT123456789_32chr!");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("CarRentalAPI");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("CarRentalClient");
        _configMock.Setup(c => c["Jwt:ExpirationMinutes"]).Returns("180");

        _authService = new AuthService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _loggerMock.Object,
            _configMock.Object,
            _customerRepoMock.Object);
    }

    [Fact]
    public async Task should_return_user_id_when_user_is_created()
    {
        // Arrange
        var email = "test@email.com";
        var fullName = "Test User";
        var password = "Test123!";

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleNames.User))
            .ReturnsAsync(IdentityResult.Success);

        _customerRepoMock.Setup(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);


        // Act
        var userId = await _authService.RegisterAsync(fullName, email, password);

        // Assert
        Assert.False(string.IsNullOrEmpty(userId));
        _userManagerMock.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(u => u.Email == email && u.FullName == fullName), password), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleNames.User), Times.Once);
        _customerRepoMock.Verify(x => x.AddAsync(It.Is<Customer>(c => c.FullName == fullName), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task should_return_jwt_token_when_credentials_are_valid()
    {
        // Arrange
        var email = "test@email.com";
        var password = "Test123!";
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = email, UserName = email };

        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, password, false))
            .ReturnsAsync(SignInResult.Success);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { RoleNames.User });

        // Act
        var jwt = await _authService.LoginAsync(email, password);

        // Assert
        Assert.False(string.IsNullOrEmpty(jwt));
        Assert.Contains(".", jwt); // Simple check for JWT format (header.payload.signature)
    }

    [Fact]
    public async Task should_throw_when_user_not_found()
    {
        // Arrange
        var email = "unknown@email.com";
        var password = "AnyPass";

        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((ApplicationUser?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidCredentialsException>(() => _authService.LoginAsync(email, password));
    }

    [Fact]
    public async Task should_throw_when_password_is_invalid()
    {
        // Arrange
        var email = "test@email.com";
        var password = "WrongPass";
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = email, UserName = email };

        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidCredentialsException>(() => _authService.LoginAsync(email, password));

        Assert.Equal("Invalid credentials.", ex.Message);
    }
    [Fact]
    public async Task should_throw_when_user_creation_fails()
    {
        // Arrange
        var email = "fail@email.com";
        var fullName = "Fail User";
        var password = "Fail123!";

        var identityErrors = new List<IdentityError>
    {
        new IdentityError { Description = "Email is already taken" },
        new IdentityError { Description = "Password is too weak" }
    };

        var failedResult = IdentityResult.Failed(identityErrors.ToArray());

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(failedResult);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ApplicationException>(() => _authService.RegisterAsync(fullName, email, password));

        Assert.Contains("Email is already taken", ex.Message);
        Assert.Contains("Password is too weak", ex.Message);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Email is already taken")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

}
