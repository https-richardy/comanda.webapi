namespace Comanda.WebApi.Services;

public sealed class OrderHistoryFormatter : IOrderHistoryFormatter
{
    public string Format(IEnumerable<Order> orderHistory)
    {
        var stringBuilder = new StringBuilder();

        if (!orderHistory.Any())
        {
            stringBuilder.AppendLine("No orders found for this customer.");
            return stringBuilder.ToString();
        }

        foreach (var order in orderHistory)
        {
            stringBuilder.AppendLine($"Order Date: {order.Date:yyyy-MM-dd}");
            stringBuilder.AppendLine($"Total Amount: {order.Total:C}");
            stringBuilder.AppendLine($"Status: {order.Status}");
            stringBuilder.AppendLine("Items:");

            foreach (var item in order.Items)
            {
                stringBuilder.AppendLine($"- {item.Product.Title} x{item.Quantity} ({item.Product.Price:C} each)");

                if (item.Additionals.Any())
                {
                    stringBuilder.AppendLine("  Additionals:");
                    foreach (var additional in item.Additionals)
                    {
                        stringBuilder.AppendLine($"    - {additional.Additional.Name}: {additional.Additional.Price:C} x{additional.Quantity}");
                    }
                }

                if (item.UnselectedIngredients.Any())
                {
                    stringBuilder.AppendLine("  Removed Ingredients:");
                    foreach (var ingredient in item.UnselectedIngredients)
                    {
                        stringBuilder.AppendLine($"    - {ingredient.Ingredient.Name}");
                    }
                }
            }
        }

        return stringBuilder.ToString();
    }
}