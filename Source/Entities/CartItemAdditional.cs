namespace Comanda.WebApi.Entities;

public sealed class CartItemAdditional : Entity
{
    public int Quantity { get; set; }
    public Additional Additional { get; set; }

    public CartItemAdditional()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public CartItemAdditional(Additional additional, int quantity)
    {
        Additional = additional;
        Quantity = quantity;
    }
}