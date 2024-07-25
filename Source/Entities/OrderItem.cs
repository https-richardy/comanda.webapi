namespace Comanda.WebApi.Entities;

public sealed class OrderItem : Entity
{
    public int Quantity { get; set; }
    public Product Product { get; set; }

    public OrderItem()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public OrderItem(int quantity, Product product)
    {
        Quantity = quantity;
        Product = product;
    }
}