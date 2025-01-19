namespace Comanda.WebApi.Payloads;

public record class FormattedIngredient
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int StandardQuantity { get; init; }
    public bool IsMandatory { get; init; }

    public FormattedIngredient(int id, string name, int standardQuantity, bool isMandatory)
    {
        Id = id;
        Name = name;
        StandardQuantity = standardQuantity;
        IsMandatory = isMandatory;
    }

    public static implicit operator FormattedIngredient(ProductIngredient productIngredient)
    {
        var ingredient = productIngredient.Ingredient;

        return new FormattedIngredient(
            id: productIngredient.Id,
            name: ingredient.Name,
            standardQuantity: productIngredient.StandardQuantity,
            isMandatory: productIngredient.IsMandatory
        );
    }
}
