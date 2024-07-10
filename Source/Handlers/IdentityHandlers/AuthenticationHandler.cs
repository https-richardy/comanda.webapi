namespace Comanda.WebApi.Handlers;

public sealed class AuthenticationHandler(
    UserManager<Account> userManager,
    IJwtService jwtService
) : IRequestHandler<AuthenticationCredentials, Response<AuthenticationResponse>>
{
    # pragma warning disable CS8604
    public async Task<Response<AuthenticationResponse>> Handle(AuthenticationCredentials request, CancellationToken cancellationToken)
    {
        var account = await userManager.FindByEmailAsync(request.Email);

        if (account is null)
            return Response<AuthenticationResponse>.Error(401, "invalid email or password.");

        if (!await userManager.CheckPasswordAsync(account, request.Password))
            return Response<AuthenticationResponse>.Error(401, "invalid email or password.");

        var roles = await userManager.GetRolesAsync(account);
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
        var token = jwtService.GenerateToken(claimsIdentity);

        var response = new AuthenticationResponse { Token = token };
        return new Response<AuthenticationResponse>(data: response, statusCode: 200 , message: "authentication successful.");
    }
}