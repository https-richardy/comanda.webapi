namespace Comanda.WebApi.Services;

public interface ICheckoutManager
{
    Task<CheckoutResponse> CreateCheckoutSessionAsync(Cart cart, Address address);
    Task<Session> GetSessionAsync(string sessionId);
}