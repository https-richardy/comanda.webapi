namespace Comanda.WebApi.Entities;

public sealed class Payment : Entity
{
    public string PaymentIntentId { get; set; }
    public decimal Amount { get; set; }
    public Order Order { get; set; }

    public Payment()
    {

    }

    public Payment(string paymentIntentId, decimal amount, Order order)
    {
        PaymentIntentId = paymentIntentId;
        Amount = amount;
        Order = order;
    }
}