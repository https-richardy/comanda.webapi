namespace Comanda.WebApi.Exceptions;

public sealed class CouponAlreadyUsedException : Exception
{
    public CouponAlreadyUsedException(string couponCode)
        : base($"Coupon with code {couponCode} already used.")
    {

    }
}