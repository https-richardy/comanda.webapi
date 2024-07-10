namespace Comanda.WebApi.Services;

public sealed class UserContextService(
    IHttpContextAccessor contextAccessor,
    ILogger<UserContextService> logger
) : IUserContextService
{
    public string? GetCurrentUserIdentifier()
    {
        var user = GetCurrentUserClaimsPrincipal();

        if (user is null)
            return null;

        var userIdentifier = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = user.FindFirstValue(ClaimTypes.Name);

        logger.LogDebug("Current user in context: {UserName}, {UserId}", userName, userIdentifier);

        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public ClaimsPrincipal? GetCurrentUserClaimsPrincipal()
    {
        var user = contextAccessor.HttpContext?.User;

        if (user is null)
        {
            logger.LogDebug("No user found in the context");
            return null;
        }

        return user;
    }
}