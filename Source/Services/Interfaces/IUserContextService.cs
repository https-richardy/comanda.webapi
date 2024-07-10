namespace Comanda.WebApi.Services;

public interface IUserContextService
{
    string? GetCurrentUserIdentifier();
    ClaimsPrincipal? GetCurrentUserClaimsPrincipal();
}