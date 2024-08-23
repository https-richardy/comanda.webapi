namespace Comanda.WebApi.Entities;

public sealed class ConfirmationToken : Entity
{
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is ConfirmationToken otherToken)
            return Token == otherToken.Token;

        return false;
    }

    public override int GetHashCode()
    {
        return Token != null ? Token.GetHashCode() : 0;
    }

    public static bool operator ==(ConfirmationToken left, string right)
    {
        if (left is null)
            return right is null;

        return left.Token == right;
    }

    public static bool operator !=(ConfirmationToken left, string right)
    {
        return !(left == right);
    }
}