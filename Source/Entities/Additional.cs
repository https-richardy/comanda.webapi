namespace Comanda.WebApi.Entities;

public sealed class Additional : Entity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Category Category { get; set; }

    public Additional()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public Additional(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}