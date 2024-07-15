namespace Comanda.WebApi.Services;

public interface IAuthenticationService
{
    Task<bool> ValidateCredentialsAsync(AuthenticationCredentials credentials);
    Task<ClaimsIdentity> BuildClaimsIdentity(Account user);
}