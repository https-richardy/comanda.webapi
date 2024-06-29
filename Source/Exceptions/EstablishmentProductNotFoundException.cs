namespace Comanda.WebApi.Exceptions;

public sealed class EstablishmentProductNotFoundException : Exception
{
    public EstablishmentProductNotFoundException(int establishmentId, int productId)
        : base($"Establishment with ID {establishmentId} does not have product with ID {productId}.")
    {

    }
}