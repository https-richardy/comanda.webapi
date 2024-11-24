namespace Comanda.TestingSuite.Integration.Services;

public sealed class CouponServiceIntegrationTest : IntegrationFixture<ComandaDbContext>
{
    [Fact(DisplayName = "AddCouponAsync should add coupon to database")]
    public async Task AddCouponAsyncShouldAddCouponToDatabase()
    {
        var couponService = ServiceProvider.GetRequiredService<ICouponService>();
        var coupon = Fixture
            .Build<Coupon>()
            .Create();

        await couponService.AddCouponAsync(coupon);
        var result = await DbContext.Coupons.FirstOrDefaultAsync(coupon => coupon.Code == coupon.Code);

        Assert.NotNull(result);

        Assert.Equal(coupon.Code, result.Code);
        Assert.Equal(coupon.Type, result.Type);
        Assert.Equal(coupon.Discount, result.Discount);
        Assert.Equal(coupon.IsActive, result.IsActive);
        Assert.Equal(coupon.ExpirationDate, result.ExpirationDate);
    }

    [Fact(DisplayName = "GetCouponByCodeAsync should return null when code does not exist")]
    public async Task GetCouponByCodeAsyncShouldReturnNullWhenCodeDoesNotExist()
    {
        var couponService = ServiceProvider.GetRequiredService<ICouponService>();
        var result = await couponService.GetCouponByCodeAsync("TESTCOUPONCODE");

        Assert.Null(result);
    }

    [Fact(DisplayName = "GetCouponByCodeAsync should return coupon when code exists")]
    public async Task GetCouponByCodeAsyncShouldReturnCouponWhenCodeExists()
    {
        var couponService = ServiceProvider.GetRequiredService<ICouponService>();
        var coupon = Fixture.Create<Coupon>();

        await DbContext.Coupons.AddAsync(coupon);
        await DbContext.SaveChangesAsync();

        var result = await couponService.GetCouponByCodeAsync(coupon.Code);

        Assert.NotNull(result);

        Assert.Equal(coupon.Code, result.Code);
        Assert.Equal(coupon.Type, result.Type);
        Assert.Equal(coupon.Discount, result.Discount);
        Assert.Equal(coupon.IsActive, result.IsActive);
        Assert.Equal(coupon.ExpirationDate, result.ExpirationDate);
    }

    [Fact(DisplayName = "ApplyDiscountAsync should throw CouponInvalidException when coupon does not exist")]
    public async Task ApplyDiscountAsyncShouldThrowCouponInvalidExceptionWhenCouponDoesNotExists()
    {
        var couponService = ServiceProvider.GetRequiredService<ICouponService>();

        var customer = Fixture.Create<Customer>();
        var exception = await Assert.ThrowsAsync<CouponInvalidException>(() => couponService.ApplyDiscountAsync(customer, "INVALIDCODE", 100m));
    }

    [Fact(DisplayName = "ApplyDiscountAsync should throw CouponAlreadyUsedException when coupon is already used")]
    public async Task ApplyDiscountAsyncShouldThrowCouponAlreadyUsedExceptionWhenCouponIsAlreadyUsed()
    {
        var couponService = ServiceProvider.GetRequiredService<ICouponService>();

        var customer = Fixture.Create<Customer>();
        var coupon = Fixture
            .Build<Coupon>()
            .With(coupon => coupon.Code, "TESTCOUPONCODE")
            .With(coupon => coupon.ExpirationDate, DateTime.UtcNow.AddDays(2))
            .With(coupon => coupon.IsActive, true)
            .Create();

        await DbContext.Coupons.AddAsync(coupon);
        await DbContext.SaveChangesAsync();

        /* simulating that the customer has already used the coupon. */
        var usedCoupon = new CouponUsage { Coupon = coupon, Customer = customer };

        await DbContext.CouponUsages.AddAsync(usedCoupon);
        await DbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<CouponAlreadyUsedException>(() => couponService.ApplyDiscountAsync(customer, coupon.Code, 100m));

        Assert.Equal($"Coupon with code {coupon.Code} already used.", exception.Message);
    }

    [Theory(DisplayName = "ApplyDiscountAsync should apply percentage coupon discount successfully")]
    [InlineData(10, 100, 90)]  /* 10% discount on 100 = 90 */
    [InlineData(20, 100, 80)]  /* 20% discount on 100 = 80 */
    [InlineData(50, 200, 100)] /* 50% discount on 200 = 100 */
    [InlineData(30, 300, 210)] /* 30% discount on 300 = 210 */
    [InlineData(5, 100, 95)]   /* 5% discount on 100 = 95 */
    public async Task ApplyDiscountAsyncShouldApplyCouponSuccessfully(decimal discount, decimal amount, decimal expected)
    {
        var couponService = ServiceProvider.GetRequiredService<ICouponService>();

        var customer = Fixture.Create<Customer>();
        var coupon = Fixture
            .Build<Coupon>()
            .With(coupon => coupon.Code, "TESTCOUPONCODE")
            .With(coupon => coupon.ExpirationDate, DateTime.UtcNow.AddDays(2))
            .With(coupon => coupon.Type, ECouponType.Percentage)
            .With(coupon => coupon.IsActive, true)
            .With(coupon => coupon.Discount, discount)
            .Create();

        await DbContext.Coupons.AddAsync(coupon);
        await DbContext.SaveChangesAsync();

        var result = await couponService.ApplyDiscountAsync(customer, coupon.Code, amount);

        Assert.Equal(expected, result);
    }

    [Theory(DisplayName = "ApplyDiscountAsync should apply fixed coupon discount successfully")]
    [InlineData(25, 100, 75)]  /* 25 discount on 100 = 75 */
    [InlineData(50, 100, 50)]  /* 50 discount on 100 = 50 */
    [InlineData(20, 200, 180)] /* 20 discount on 200 = 180 */
    [InlineData(10, 50, 40)]   /* 10 discount on 50 = 40 */
    [InlineData(5, 100, 95)]   /* 5 discount on 100 = 95 */
    public async Task ApplyDiscountAsyncShouldApplyFixedCouponSuccessfully(decimal discount, decimal amount, decimal expected)
    {
        var couponService = ServiceProvider.GetRequiredService<ICouponService>();

        var customer = Fixture.Create<Customer>();
        var coupon = Fixture
            .Build<Coupon>()
            .With(coupon => coupon.Code, "FIXEDCOUPONCODE")
            .With(coupon => coupon.ExpirationDate, DateTime.UtcNow.AddDays(2))
            .With(coupon => coupon.Type, ECouponType.Fixed)
            .With(coupon => coupon.IsActive, true)
            .With(coupon => coupon.Discount, discount)
            .Create();

        await DbContext.Coupons.AddAsync(coupon);
        await DbContext.SaveChangesAsync();

        var result = await couponService.ApplyDiscountAsync(customer, coupon.Code, amount);

        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "UpdateCouponAsync should update coupon in the database")]
    public async Task UpdateCouponAsyncShouldUpdateCouponInDatabase()
    {
        var couponService = ServiceProvider.GetRequiredService<ICouponService>();
        var coupon = Fixture.Create<Coupon>();

        await DbContext.Coupons.AddAsync(coupon);
        await DbContext.SaveChangesAsync();

        coupon.Discount = 50;
        await couponService.UpdateCouponAsync(coupon);

        var updatedCoupon = await DbContext.Coupons.FirstOrDefaultAsync(coupon => coupon.Id == coupon.Id);

        Assert.NotNull(updatedCoupon);
        Assert.Equal(50, updatedCoupon.Discount);
    }

    [Fact(DisplayName = "DeleteCouponAsync should remove coupon from the database")]
    public async Task DeleteCouponAsyncShouldRemoveCouponFromDatabase()
    {
        var couponService = ServiceProvider.GetRequiredService<ICouponService>();
        var coupon = Fixture.Create<Coupon>();

        await DbContext.Coupons.AddAsync(coupon);
        await DbContext.SaveChangesAsync();

        await couponService.DeleteCouponAsync(coupon);

        var deletedCoupon = await DbContext.Coupons.FirstOrDefaultAsync(coupon => coupon.Id == coupon.Id);

        Assert.NotNull(deletedCoupon);
        Assert.True(deletedCoupon.IsDeleted);
    }
}