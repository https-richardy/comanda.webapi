namespace Comanda.WebApi.Entities;

public sealed class Coupon : Entity
{
    public string Code { get; set; }
    public decimal Discount { get; set; }
    public bool IsActive => ExpirationDate > DateTime.Now;

    public DateTime ExpirationDate { get; set; }
    public ECouponType Type { get; set; }

    public Coupon()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public Coupon(string code, decimal discount, DateTime expirationDate, ECouponType type)
    {
        Code = code;
        Discount = discount;
        ExpirationDate = expirationDate;
        Type = type;
    }

    public decimal ApplyDiscount(decimal value)
    {
        return Type switch
        {
            ECouponType.Percentage => value - (value * Discount / 100),
            ECouponType.Fixed => value - Discount,

            _ => 0m /* return 0 for unsupported types */
        };
    }
}