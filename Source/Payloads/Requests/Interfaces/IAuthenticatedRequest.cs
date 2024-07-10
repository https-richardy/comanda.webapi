namespace Comanda.WebApi.Payloads;

/// <summary>
/// Interface to ensure that requests requiring user authentication contain a UserId.
/// </summary>
/// <remarks>
/// Implementing this interface in request classes and records ensures that the UserId is included, 
/// providing a contract that the request is made by an authenticated user.
/// </remarks>
public interface IAuthenticatedRequest
{
    string UserId { get; }
}