namespace Comanda.WebApi.Entities;

public sealed class EstablishmentOwner : Entity
{
    public Establishment Establishment { get; set; }
    public Account Account { get; set; }

    public EstablishmentOwner()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public EstablishmentOwner(Establishment establishment, Account account)
    {
        Establishment = establishment;
        Account = account;
    }
}