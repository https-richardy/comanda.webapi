namespace Comanda.WebApi.Exceptions;

public sealed class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException(int customerId)
        : base($"Customer with ID {customerId} not found.")
    {
    }
}