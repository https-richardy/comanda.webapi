namespace Comanda.WebApi.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<Account> _userManager;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        UserManager<Account> userManager,
        ILogger<AuthenticationService> logger
    )
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<bool> ValidateCredentialsAsync(AuthenticationCredentials credentials)
    {
        var user = await _userManager.FindByEmailAsync(credentials.Email);
        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not found", credentials.Email);
            return false;
        }

        var result = await _userManager.CheckPasswordAsync(user, credentials.Password);
        if (!result)
        {
            _logger.LogWarning("Invalid credentials for user with email {Email}", credentials.Email);
            return false;
        }

        return true;
    }

    #pragma warning disable CS8604
    public async Task<ClaimsIdentity> BuildClaimsIdentity(Account user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var claimsIdentity = new ClaimsIdentity(claims);
        return claimsIdentity;
    }
}