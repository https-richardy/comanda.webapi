namespace Comanda.WebApi.Handlers;

public sealed class AuthenticationHandler(
    UserManager<Account> userManager,
    IAuthenticationService authenticationService,
    IJwtService jwtService
) : IRequestHandler<AuthenticationCredentials, Response<AuthenticationResponse>>
{
    # pragma warning disable CS8604
    public async Task<Response<AuthenticationResponse>> Handle(
        AuthenticationCredentials request,
        CancellationToken cancellationToken
    )
    {
        var validCredentials = await authenticationService.ValidateCredentialsAsync(request);
        if (validCredentials is false)
            return new Response<AuthenticationResponse>(
                data: null,
                statusCode: StatusCodes.Status401Unauthorized,
                message: "invalid credentials."
            );

        var account = await userManager.FindByEmailAsync(request.Email);
        var claimsIdentity = await authenticationService.BuildClaimsIdentity(account);

        var token = jwtService.GenerateToken(claimsIdentity);

        var response = new AuthenticationResponse { Token = token };
        return new Response<AuthenticationResponse>(
            data: response,
            statusCode: StatusCodes.Status200OK,
            message: "authentication was successful."
        );
    }
}