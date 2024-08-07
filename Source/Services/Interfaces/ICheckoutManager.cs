namespace Comanda.WebApi.Services;

public interface ICheckoutManager
{
    Task<Session> CreateCheckoutSessionAsync(Cart cart, Address address);
    Task<Session> GetSessionAsync(string sessionId);
}