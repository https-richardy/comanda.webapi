namespace Comanda.WebApi.Exceptions;

public sealed class CouponInvalidException : Exception
{
    public CouponInvalidException(string couponCode)
        : base($"Coupon with code {couponCode} is invalid. Invalid or expired coupon.")
    {

    }
}