namespace Comanda.WebApi.Entities;

public sealed class CouponUsage : Entity
{
    public Coupon Coupon { get; set; }
    public Customer Customer { get; set; }

    public CouponUsage()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public CouponUsage(Coupon coupon, Customer customer)
    {
        Coupon = coupon;
        Customer = customer;
    }
}