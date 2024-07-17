namespace Comanda.WebApi.Handlers;

public sealed class AuthenticationHandler(
    UserManager<Account> userManager,
    IAuthenticationService authenticationService,
    IJwtService jwtService
) :
    IRequestHandler<AuthenticationCredentials, Response<AuthenticationResponse>>
{
    private readonly UserManager<Account> _userManager = userManager;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IJwtService _jwtService = jwtService;

    # pragma warning disable CS8604
    public async Task<Response<AuthenticationResponse>> Handle(
        AuthenticationCredentials request,
        CancellationToken cancellationToken
    )
    {
        var validCredentials = await _authenticationService.ValidateCredentialsAsync(request);
        if (validCredentials is false)
            return new Response<AuthenticationResponse>(
                data: null,
                statusCode: StatusCodes.Status401Unauthorized,
                message: "invalid credentials."
            );

        var account = await _userManager.FindByEmailAsync(request.Email);
        var claimsIdentity = await _authenticationService.BuildClaimsIdentity(account);

        var token = _jwtService.GenerateToken(claimsIdentity);

        var response = new AuthenticationResponse { Token = token };
        return new Response<AuthenticationResponse>(
            data: response,
            statusCode: StatusCodes.Status200OK,
            message: "authentication was successful."
        );
    }
}