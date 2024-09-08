namespace Comanda.WebApi.Data.Repositories;

public sealed class CouponRepository(ComandaDbContext dbContext) :
    MinimalRepository<Coupon, ComandaDbContext>(dbContext), ICouponRepository
{
    #pragma warning disable CS8603
    public async Task<Coupon> GetCouponByCodeAsync(string couponCode)
    {
        return await _dbContext.Coupons
            .Where(coupon => coupon.Code == couponCode)
            .FirstOrDefaultAsync();
    }

    public async Task<Coupon> GetValidCouponAsync(string couponCode)
    {
        return await _dbContext.Coupons
            .Where(coupon => coupon.Code == couponCode && coupon.IsActive == true)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasUserUsedCouponAsync(string couponCode, int customerId)
    {
        return await _dbContext.CouponUsages
            .AnyAsync(usage => usage.Customer.Id == customerId && usage.Coupon.Code == couponCode);
    }

    public async Task AddCouponUsageAsync(Coupon coupon, Customer customer)
    {
        var usage = new CouponUsage
        {
            Coupon = coupon,
            Customer = customer
        };

        await _dbContext.CouponUsages.AddAsync(usage);
        await _dbContext.SaveChangesAsync();
    }
}