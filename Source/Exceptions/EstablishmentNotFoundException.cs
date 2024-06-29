namespace Comanda.WebApi.Exceptions;

public sealed class EstablishmentNotFoundException : Exception
{
    public EstablishmentNotFoundException(int establishmentId)
        : base($"Establishment with ID {establishmentId} not found.")
    {

    }
}