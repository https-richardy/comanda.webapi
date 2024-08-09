namespace Comanda.WebApi.Services;

public interface IRefundManager
{
    Task<Stripe.Refund> RefundAsync(string paymentIntentId, decimal amount);
    Task<Stripe.Refund> RefundOrderAsync(Order order);
}