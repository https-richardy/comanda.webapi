namespace Comanda.WebApi.Entities;

public sealed class CartItem : Entity
{
    public int Quantity { get; set; }
    public Product Product { get; set; }

    public CartItem()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public CartItem(int quantity, Product product)
    {
        Quantity = quantity;
        Product = product;
    }
}