namespace Comanda.WebApi.Entities;

public sealed class UnselectedIngredient : Entity
{
    public Ingredient Ingredient { get; set; }
    public CartItem CartItem { get; set; }

    public UnselectedIngredient()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public UnselectedIngredient(Ingredient ingredient, CartItem cartItem)
    {
        Ingredient = ingredient;
        CartItem = cartItem;
    }
}