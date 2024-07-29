namespace Comanda.WebApi.Entities;

public sealed class Cart : Entity
{
    public decimal Total => Items.Sum(item => item.Total);

    public Customer Customer { get; set; }
    public ICollection<CartItem> Items { get; set; } = [];

    public Cart()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public Cart(Customer customer)
    {
        Customer = customer;
    }

    public void AddItem(Product product, int quantity)
    {
        var existingItem = Items.FirstOrDefault(item => item.Product.Id == product.Id);

        if (existingItem != null)
            existingItem.Quantity += quantity;
        else
            Items.Add(new CartItem(quantity, product));
    }

    public void RemoveItem(Product product)
    {
        var item = Items.FirstOrDefault(item => item.Product.Id == product.Id);
        if (item != null)
            Items.Remove(item);
    }
}