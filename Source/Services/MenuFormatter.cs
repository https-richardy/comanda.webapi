namespace Comanda.WebApi.Services;

public sealed class MenuFormatterService : IMenuFormatter
{
    public string Format(IEnumerable<Product> products)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Menu:");
        foreach (var product in products)
        {
            stringBuilder.AppendLine($"- {product.Title}: {product.Price:C}");
            stringBuilder.AppendLine($"  Description: {product.Description}");

            if (product.Ingredients.Any())
            {
                stringBuilder.AppendLine("  Ingredients:");
                foreach (var ingredient in product.Ingredients)
                {
                    stringBuilder.AppendLine($"    - {ingredient.Ingredient.Name}");
                }
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}