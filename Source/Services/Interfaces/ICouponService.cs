namespace Comanda.WebApi.Services;

public interface ICouponService
{
    Task AddCouponAsync(Coupon coupon);
    Task UpdateCouponAsync(Coupon coupon);
    Task DeleteCouponAsync(Coupon coupon);
    Task<Coupon?> GetCouponByCodeAsync(string couponCode);
    Task<decimal> ApplyDiscountAsync(Customer customer, string couponCode, decimal value);
}