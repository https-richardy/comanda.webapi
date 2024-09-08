namespace Comanda.WebApi.Data.Repositories;

public interface ICouponRepository : IMinimalRepository<Coupon>
{
    Task AddCouponUsageAsync(Coupon coupon, Customer customer);
    Task<Coupon> GetValidCouponAsync(string couponCode);
    Task<Coupon> GetCouponByCodeAsync(string couponCode);
    Task<bool> HasUserUsedCouponAsync(string couponCode, int customerId);
}