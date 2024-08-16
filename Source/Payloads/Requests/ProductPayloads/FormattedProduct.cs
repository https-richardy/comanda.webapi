namespace Comanda.WebApi.Payloads;

public sealed record class FormattedProduct
{
    public int Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Image { get; init; }
    public decimal Price { get; init; }

    public Category Category { get; init; }
    public ICollection<FormattedIngredient> Ingredients { get; init; }

    public FormattedProduct(
        int id,
        string title,
        string description,
        string image,
        decimal price,
        Category category,
        ICollection<FormattedIngredient> ingredients)
    {
        Id = id;
        Title = title;
        Description = description;
        Image = image;
        Price = price;
        Category = category;
        Ingredients = ingredients;
    }

    public static implicit operator FormattedProduct(Product product)
    {
        var ingredients = product.Ingredients
            .Select(ingredient => (FormattedIngredient)ingredient)
            .ToList();

        return new FormattedProduct(
            id: product.Id,
            title: product.Title,
            description: product.Description,
            image: product.ImagePath!,
            price: product.Price,
            category: product.Category,
            ingredients: ingredients
        );
    }
}
