namespace Comanda.WebApi.Payloads;

public sealed record OrderItemFormatted
{
    public string Product { get; init; }
    public int Quantity { get; init; }

    public ICollection<OrderItemAdditionalFormatted> Additionals { get; set; } = [];
    public ICollection<UnselectedIngredientFormatted> UnselectedIngredients { get; set; } = [];

    public static implicit operator OrderItemFormatted(OrderItem orderItem)
    {
        var formattedItem = new OrderItemFormatted
        {
            Product = orderItem.Product.Title,
            Quantity = orderItem.Quantity
        };

        foreach (var additional in orderItem.Additionals)
        {
            formattedItem.Additionals.Add(additional);
        }

        foreach (var unselectedIngredient in orderItem.UnselectedIngredients)
        {
            formattedItem.UnselectedIngredients.Add(unselectedIngredient);
        }

        return formattedItem;
    }
}