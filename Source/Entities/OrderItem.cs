namespace Comanda.WebApi.Entities;

public sealed class OrderItem : Entity
{
    public int Quantity { get; set; }
    public decimal Total => CalculateTotal();

    public Product Product { get; set; }
    public ICollection<OrderItemAdditional> Additionals { get; set; } = [];
    public ICollection<UnselectedIngredient> UnselectedIngredients { get; set; } = [];

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

    private decimal CalculateTotal()
    {
        /* Calculates the total cost of all the add-ons applied to the product. */
        return (Additionals.Sum(item => item.Additional.Price * item.Quantity) + Product.Price) * Quantity;
    }
}