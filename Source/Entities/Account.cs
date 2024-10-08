namespace Comanda.WebApi.Entities;

public sealed class Account : IdentityUser
{
    public ConfirmationToken? ConfirmationToken { get; set; } = null!;

    public Account()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }
}