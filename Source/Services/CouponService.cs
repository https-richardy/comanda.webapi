namespace Comanda.WebApi.Services;

public sealed class CouponService(ICouponRepository couponRepository, ILogger<CouponService> logger) : ICouponService
{
    public async Task<Coupon?> GetCouponByCodeAsync(string couponCode)
    {
        var coupon = await couponRepository.GetCouponByCodeAsync(couponCode);
        return coupon;
    }

    public async Task AddCouponAsync(Coupon coupon)
    {
        await couponRepository.SaveAsync(coupon);
        logger.LogInformation("Coupon with code {code} saved.", coupon.Code);
    }

    public async Task UpdateCouponAsync(Coupon coupon)
    {
        await couponRepository.UpdateAsync(coupon);
        logger.LogInformation("Coupon with code {code} updated.", coupon.Code);
    }

    public async Task DeleteCouponAsync(Coupon coupon)
    {
        await couponRepository.DeleteAsync(coupon);
        logger.LogInformation("Coupon with code {code} deleted.", coupon.Code);
    }

    public async Task<decimal> ApplyDiscountAsync(Customer customer, string couponCode, decimal value)
    {
        var coupon = await couponRepository.GetValidCouponAsync(couponCode);
        if (coupon is null)
        {
            logger.LogWarning("Coupon with code {code} not found", couponCode);
            throw new InvalidOperationException($"Invalid or expired coupon."); // TODO: throw custom expcetion.
        }

        var hasUsed = await couponRepository.HasUserUsedCouponAsync(couponCode, customer.Id);
        if (hasUsed)
        {
            logger.LogWarning("Coupon with code {code} already used", couponCode);
            throw new InvalidOperationException($"Coupon with code {couponCode} already used."); // TODO: throw custom expcetion.
        }

        await couponRepository.AddCouponUsageAsync(coupon, customer);
        return coupon.ApplyDiscount(value);
    }
}
