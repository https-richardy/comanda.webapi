namespace Comanda.WebApi.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<Account> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        UserManager<Account> userManager,
        IJwtService jwtService,
        ILogger<AuthenticationService> logger
    )
    {
        _userManager = userManager;
        _jwtService = jwtService;
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

    public async Task<string> GenerateTokenAsync(ClaimsIdentity claimsIdentity)
    {
        /* Executes the synchronous call in a separate thread to maintain asynchronicity */
        return await Task.Run(() => _jwtService.GenerateToken(claimsIdentity));
    }
}