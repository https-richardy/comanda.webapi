namespace Comanda.WebApi.Payloads;

public sealed record UnselectedIngredientFormatted
{
    public string Name { get; init; }

    public static implicit operator UnselectedIngredientFormatted(UnselectedIngredient item)
    {
        return new UnselectedIngredientFormatted
        {
            Name = item.Ingredient.Name
        };
    }
}