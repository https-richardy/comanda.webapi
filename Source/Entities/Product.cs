namespace Comanda.WebApi.Entities;

public sealed class Product : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public decimal Price { get; set; }

    public Category Category { get; set; }

    public Product()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public Product(string title, string description, string imagePath, decimal price, Category category)
    {
        Title = title;
        Description = description;
        ImagePath = imagePath;
        Price = price;
        Category = category;
    }
}