namespace Comanda.WebApi.Payloads;

public sealed record ProfileInformation
{
    public string Name { get; init; }
    public string Email { get; init; }

    public static implicit operator ProfileInformation(Account account)
    {
        return new ProfileInformation
        {
            Name = account.UserName!,
            Email = account.Email!
        };
    }
}