namespace Comanda.WebApi.Entities;

public sealed class Establishment : Entity
{
    public string Name { get; set; }

    public Address Address { get; set; }
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];

    public Establishment()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }
}