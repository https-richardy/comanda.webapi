namespace Comanda.WebApi.Entities;

public sealed class OrderItemAdditional : Entity
{
    public int Quantity { get; set; }
    public Additional Additional { get; set; }

    public OrderItemAdditional()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public OrderItemAdditional(Additional additional, int quantity)
    {
        Additional = additional;
        Quantity = quantity;
    }
}