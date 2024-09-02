using Microsoft.Extensions.Options;

namespace Comanda.TestingSuite.UnitTests.Services;

public sealed class AuthenticationServiceTests
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IFixture _fixture;
    private readonly Mock<UserManager<Account>> _userManagerMock;
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock;

    public AuthenticationServiceTests()
    {
        _userManagerMock = new Mock<UserManager<Account>>(
            new Mock<IUserStore<Account>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<Account>>().Object,
            new IUserValidator<Account>[0],
            new IPasswordValidator<Account>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<Account>>>().Object
        );

        _loggerMock = new Mock<ILogger<AuthenticationService>>();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _authenticationService = new AuthenticationService(
            _userManagerMock.Object,
            _loggerMock.Object
        );
    }

    [Fact(DisplayName = "Should handle concurrent user login attempts")]
    public async Task ShouldHandleConcurrentUserLoginAttempts()
    {
        var credentials = _fixture.Create<AuthenticationCredentials>();

        var task1 = _authenticationService.ValidateCredentialsAsync(credentials);
        var task2 = _authenticationService.ValidateCredentialsAsync(credentials);

        await Task.WhenAll(task1, task2);

        Assert.Equal(task1.Result, task2.Result);
    }

    [Fact(DisplayName = "Should return true when valid credentials are provided")]
    public async Task ShouldReturnFalseWhenInvalidCredentialsAreProvided()
    {
        var invalidCredentials = new AuthenticationCredentials
        {
            Email = "invalid.email@email.com",
            Password = "invalid_password"
        };

        _userManagerMock.Setup(manager => manager.FindByEmailAsync(invalidCredentials.Email))
            .ReturnsAsync((Account)null!);

        var result = await _authenticationService.ValidateCredentialsAsync(invalidCredentials);

        Assert.False(result);
    }

    [Fact(DisplayName = "Should return false when password is incorrect")]
    public async Task ShouldReturnFalseWhenPasswordIsIncorrect()
    {
        var existingEmail = "existing.email@email.com";
        var credentials = new AuthenticationCredentials
        {
            Email = existingEmail,
            Password = "incorrect_password"
        };

        var user = _fixture.Create<Account>();

        _userManagerMock.Setup(manager => manager.FindByEmailAsync(credentials.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(manager => manager.CheckPasswordAsync(user, credentials.Password))
            .ReturnsAsync(false);

        var result = await _authenticationService.ValidateCredentialsAsync(credentials);

        Assert.False(result);
    }

    [Fact(DisplayName = "Should return false when user is not active")]
    public async Task ShouldReturnClaimsIdentityWithAllRolesWhenUserHasMultipleRoles()
    {
        var user = _fixture.Create<Account>();
        var roles = new List<string> { "Admin", "User", "Manager" };

        _userManagerMock.Setup(manager => manager.GetRolesAsync(user))
            .ReturnsAsync(roles);

        var claimsIdentity = await _authenticationService.BuildClaimsIdentity(user);

        Assert.NotNull(claimsIdentity);
        Assert.Equal(roles.Count, claimsIdentity.Claims.Count(claim => claim.Type == ClaimTypes.Role));

        foreach (var role in roles)
        {
            Assert.Contains(claimsIdentity.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == role);
        }
    }

    [Fact(DisplayName = "Should return ClaimsIdentity without role claims when user has no roles")]
    public async Task ShouldReturnClaimsIdentityWithoutRoleClaimsWhenUserHasNoRoles()
    {
        var user = _fixture.Create<Account>();
        var roles = new List<string>();

        _userManagerMock.Setup(manager => manager.GetRolesAsync(user))
            .ReturnsAsync(roles);

        var claimsIdentity = await _authenticationService.BuildClaimsIdentity(user);

        Assert.NotNull(claimsIdentity);
        Assert.DoesNotContain(claimsIdentity.Claims, claim => claim.Type == ClaimTypes.Role);
    }
}