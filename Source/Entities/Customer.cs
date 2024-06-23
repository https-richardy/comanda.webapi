namespace Comanda.WebApi.Entities;

public sealed class Customer : Entity
{
    public string FullName { get; set; }

    public Account Account { get; set; }
    public ICollection<Address> Addresses { get; set; }
    public ICollection<Order> Orders { get; set; }

    public Customer()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }
}