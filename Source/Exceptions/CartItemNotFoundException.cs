namespace Comanda.WebApi.Exceptions;

public sealed class CartItemNotFoundException : Exception
{
    public CartItemNotFoundException(int productId)
        : base($"Cart item with product ID {productId} not found.")
    {

    }
}