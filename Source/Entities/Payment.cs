namespace Comanda.WebApi.Entities;

public sealed class Payment : Entity
{
    public decimal Amount { get; set; }
    public Order Order { get; set; }

    public Payment()
    {

    }

    public Payment(decimal amount, Order order)
    {
        Amount = amount;
        Order = order;
    }
}