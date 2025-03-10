namespace Comanda.WebApi.Services;

public interface IRefundManager
{
    Task RefundAsync(string paymentIntentId, decimal amount);
    Task RefundOrderAsync(Order order);
}