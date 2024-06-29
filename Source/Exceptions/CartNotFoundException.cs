namespace Comanda.WebApi.Exceptions;

public sealed class CartNotFoundException : Exception
{
    public CartNotFoundException(int customerId)
        : base($"Cart for customer with ID {customerId} not found.")
    {

    }
}