namespace Comanda.WebApi.Entities;

public sealed class CartItem : Entity
{
    public int Quantity { get; set; }
    public decimal Total => CalculateTotal();

    public Product Product { get; set; }
    public ICollection<CartItemAdditional> Additionals { get; set; } = [];
    public ICollection<UnselectedIngredient> UnselectedIngredients { get; set; } = [];

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

    public void AddAdditional(Additional additional, int quantity)
    {
        var existingAdditional = Additionals.FirstOrDefault(additional => additional.Additional.Id == additional.Id);

        if (existingAdditional != null)
            existingAdditional.Quantity += quantity;

        else
            Additionals.Add(new CartItemAdditional(additional, quantity));
    }

    public void RemoveAdditional(Additional additional)
    {
        var existingAdditional = Additionals.FirstOrDefault(a => a.Additional.Id == additional.Id);
        if (existingAdditional is not null)
            Additionals.Remove(existingAdditional);
    }

    private decimal CalculateTotal()
    {
        /* Calculates the total cost of all the add-ons applied to the product. */
        return (Additionals.Sum(item => item.Additional.Price * item.Quantity) + Product.Price) * Quantity;
    }
}