namespace Comanda.WebApi.Services;

public interface ICheckoutManager
{
    Task<CheckoutResponse> CreateCheckoutSessionAsync(Cart cart, Customer customer, Address address);
}