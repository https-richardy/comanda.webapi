namespace Comanda.WebApi.Payloads;

public sealed class AddressExportFormatted
{
    public string Street { get; init; }
    public string Number { get; init; }
    public string City { get; init; }
    public string Neighborhood { get; init; }
    public string PostalCode { get; init; }

    public static implicit operator AddressExportFormatted(Address address)
    {
        return new AddressExportFormatted
        {
            Street = address.Street,
            Number = address.Number,
            City = address.City,
            Neighborhood = address.Neighborhood,
            PostalCode = address.PostalCode
        };
    }
}