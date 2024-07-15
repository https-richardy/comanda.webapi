namespace Comanda.WebApi.Services;

public interface IAuthenticationService
{
    Task<bool> ValidateCredentialsAsync(AuthenticationCredentials credentials);
    Task<string> GenerateTokenAsync(ClaimsIdentity claimsIdentity);
}