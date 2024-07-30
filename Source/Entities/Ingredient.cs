namespace Comanda.WebApi.Entities;

public sealed class Ingredient : Entity
{
    public string Name { get; set; }

    public Ingredient()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public Ingredient(string name)
    {
        Name = name;
    }
}