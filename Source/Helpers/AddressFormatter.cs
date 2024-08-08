namespace Comanda.WebApi.Helpers;

public static class AddressFormatter
{
    public static string Format(Address address)
    {
        return $"{address.Street} {address.Number}, {address.City}";
    }

    public static string FormatComplete(Address address)
    {
        var formattedAddress = $"{address.Street} {address.Number}, {address.Neighborhood}, " +
                                $"{address.City} - {address.State}, {address.PostalCode}";

        if (!string.IsNullOrEmpty(address.Complement))
            formattedAddress += $", Complemento: {address.Complement}";

        if (!string.IsNullOrEmpty(address.Reference))
            formattedAddress += $", Referência: {address.Reference}";

        return formattedAddress;
    }
}