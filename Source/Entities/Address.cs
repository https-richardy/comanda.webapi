namespace Comanda.WebApi.Entities;

public class Address : Entity
{
    public string Street { get; set; }
    public string Number { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Neighborhood { get; set; }
    public string PostalCode { get; set; }
    public string? Complement { get; set; }
    public string? Reference { get; set; }

    public Address()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public Address(string street, string number, string city, string state, string neighborhood,
                   string postalCode, string? complement, string? reference)
    {
        Street = street;
        Number = number;
        City = city;
        State = state;
        Neighborhood = neighborhood;
        PostalCode = postalCode;
        Complement = complement;
        Reference = reference;
    }
}