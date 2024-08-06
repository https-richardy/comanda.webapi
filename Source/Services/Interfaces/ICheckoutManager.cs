namespace Comanda.WebApi.Services;

public interface ICheckoutManager
{
    Task<Session> CreateCheckoutSessionAsync(Cart cart);
    Task<Session> GetSessionAsync(string sessionId);
}