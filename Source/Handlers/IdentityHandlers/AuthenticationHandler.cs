namespace Comanda.WebApi.Handlers;

public sealed class AuthenticationHandler :
    IRequestHandler<AuthenticationCredentials, Response<AuthenticationResponse>>
{
    private readonly UserManager<Account> _userManager;
    private readonly IJwtService _jwtService;

    public AuthenticationHandler(UserManager<Account> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    # pragma warning disable CS8604
    public async Task<Response<AuthenticationResponse>> Handle(AuthenticationCredentials request, CancellationToken cancellationToken)
    {
        var account = await _userManager.FindByEmailAsync(request.Email);
        if (account is null)
            return new Response<AuthenticationResponse>(
                data: null,
                statusCode: StatusCodes.Status401Unauthorized,
                message: "invalid email or password."
            );

        if (!await _userManager.CheckPasswordAsync(account, request.Password))
            return new Response<AuthenticationResponse>(
                data: null,
                statusCode: StatusCodes.Status401Unauthorized,
                message: "invalid email or password."
            );

        var roles = await _userManager.GetRolesAsync(account);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.UserName),
            new Claim(ClaimTypes.Email, account.Email),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var claimsIdentity = new ClaimsIdentity(claims);
        var token = _jwtService.GenerateToken(claimsIdentity);

        var response = new AuthenticationResponse { Token = token };
        return new Response<AuthenticationResponse>(
            data: response,
            statusCode: StatusCodes.Status200OK,
            message: "authentication was successful."
        );
    }
}