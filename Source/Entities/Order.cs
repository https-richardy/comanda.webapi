namespace Comanda.WebApi.Entities;

public sealed class Order : Entity
{
    public decimal Total => OrderItems.Sum(item => item.Price);

    public Establishment Establishment { get; set; }
    public Customer Customer { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public EOrderStatus Status { get; set; }
    public DateTime Date { get; set; }

    public Order()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }
}