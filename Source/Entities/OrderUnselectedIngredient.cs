namespace Comanda.WebApi.Entities;

public sealed class OrderUnselectedIngredient : Entity
{
    public Ingredient Ingredient { get; set; }
    public OrderItem OrderItem { get; set; }

    public OrderUnselectedIngredient()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public OrderUnselectedIngredient(Ingredient ingredient, OrderItem orderItem)
    {
        Ingredient = ingredient;
        OrderItem = orderItem;
    }
}
