namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class CouponRepositoryTests : InMemoryDatabaseFixture<ComandaDbContext>
{
    private readonly ICouponRepository _repository;
    private readonly IFixture _fixture;

    public CouponRepositoryTests()
    {
        _repository = new CouponRepository(dbContext: DbContext);

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "Given a new coupon, should save successfully in the database")]
    public async Task GivenNewCoupon_ShouldSaveSuccessfullyInTheDatabase()
    {
        var coupon = _fixture.Create<Coupon>();

        await _repository.SaveAsync(coupon);
        var savedCoupon = await DbContext.Coupons.FindAsync(coupon.Id);

        Assert.NotNull(savedCoupon);

        Assert.Equal(coupon.Code, savedCoupon.Code);
        Assert.Equal(coupon.Discount, savedCoupon.Discount);
    }

    [Fact(DisplayName = "Given an exisiting coupon, should update successfully in the database")]
    public async Task GivenExisitingCoupon_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var coupon = _fixture.Create<Coupon>();
        var updatedCoupon = _fixture.Create<Coupon>();

        await _repository.SaveAsync(coupon);

        updatedCoupon.Id = coupon.Id;
        updatedCoupon.Code = coupon.Code + "-updated";

        await _repository.UpdateAsync(updatedCoupon);
        var updatedSavedCoupon = await DbContext.Coupons.FindAsync(updatedCoupon.Id);

        Assert.NotNull(updatedSavedCoupon);

        Assert.Equal(updatedCoupon.Code, updatedSavedCoupon.Code);
        Assert.Equal(updatedCoupon.Discount, updatedSavedCoupon.Discount);
        Assert.Equal(updatedCoupon.Type, updatedSavedCoupon.Type);
    }

    [Fact(DisplayName = "Given an exisiting coupon, should delete successfully in the database")]
    public async Task GivenExisitingCoupon_ShouldDeleteSuccessfullyInTheDatabase()
    {
        var coupon = _fixture.Create<Coupon>();

        await _repository.SaveAsync(coupon);
        await _repository.DeleteAsync(coupon);

        var deletedCoupon = await DbContext.Coupons.FindAsync(coupon.Id);

        Assert.Null(deletedCoupon);
    }

    [Fact(DisplayName = "Should retrieve a coupon by ID successfully")]
    public async Task ShouldRetrieveCouponByIdSuccessfully()
    {
        var coupon = _fixture.Create<Coupon>();

        await _repository.SaveAsync(coupon);
        var retrievedCoupon = await _repository.RetrieveByIdAsync(coupon.Id);

        Assert.NotNull(retrievedCoupon);

        Assert.Equal(coupon.Code, retrievedCoupon.Code);
        Assert.Equal(coupon.Discount, retrievedCoupon.Discount);
    }

    [Fact(DisplayName = "Should retrieve all coupons")]
    public async Task ShouldRetrieveAllCoupons()
    {
        var coupons = _fixture
            .CreateMany<Coupon>(3)
            .ToList();

        foreach (var coupon in coupons)
        {
            await _repository.SaveAsync(coupon);
        }

        var allCoupons = await _repository.RetrieveAllAsync();

        Assert.Equal(coupons.Count, allCoupons.Count());
        Assert.All(allCoupons, coupon => Assert.Contains(coupon, coupons));
    }

    [Fact(DisplayName = "Given a valid coupon code, should get a valid coupon")]
    public async Task GivenValidCouponCode_ShouldGetValidCoupon()
    {
        const string couponCode = "TESTCOUPON";
        var coupon = _fixture.Build<Coupon>()
            .With(coupon => coupon.Code, couponCode)
            .With(coupon => coupon.ExpirationDate, DateTime.UtcNow.AddDays(2))
            .With(coupon => coupon.IsActive, true)
            .Create();

        await _repository.SaveAsync(coupon);
        var validCoupon = await _repository.GetValidCouponAsync(couponCode);

        Assert.NotNull(validCoupon);
        Assert.Equal(couponCode, validCoupon.Code);
    }

    [Fact(DisplayName = "Should retrieve a coupon by its code")]
    public async Task ShouldRetrieveACouponByItsCode()
    {
        const string couponCode = "TESTCOUPONCODE";
        var coupon = _fixture.Build<Coupon>()
            .With(coupon => coupon.Code, couponCode)
            .Create();

        await _repository.SaveAsync(coupon);
        var retrievedCoupon = await _repository.GetCouponByCodeAsync(couponCode);

        Assert.NotNull(retrievedCoupon);
        Assert.Equal(couponCode, retrievedCoupon.Code);
        Assert.Equal(coupon.Discount, retrievedCoupon.Discount);
    }

    [Fact(DisplayName = "Should add coupon usage")]
    public async Task ShouldAddCouponUsage()
    {
        const string couponCode = "USAGE-COUPON";
        var coupon = _fixture.Build<Coupon>()
            .With(coupon => coupon.Code, couponCode)
            .Create();

        var customer = _fixture.Create<Customer>();

        await _repository.SaveAsync(coupon);
        await _repository.AddCouponUsageAsync(coupon, customer);

        var couponUsage = await DbContext.CouponUsages.FirstOrDefaultAsync(usage =>
            usage.Coupon.Id == coupon.Id && usage.Customer.Id == customer.Id
        );

        Assert.NotNull(couponUsage);

        Assert.Equal(coupon.Id, couponUsage.Coupon.Id);
        Assert.Equal(customer.Id, couponUsage.Customer.Id);
    }

    [Fact(DisplayName = "Should check if a user has used a coupon")]
    public async Task ShouldCheckIfUserHasUsedCoupon()
    {
        const string couponCode = "USEDCOUPON";
        var coupon = _fixture.Build<Coupon>()
            .With(coupon => coupon.Code, couponCode)
            .Create();

        var customer = _fixture.Create<Customer>();
        await _repository.SaveAsync(coupon);

        /* Initially, the coupon shouldn't be used */
        Assert.False(await _repository.HasUserUsedCouponAsync(couponCode, customer.Id));

        /* Simulate coupon usage */
        await _repository.AddCouponUsageAsync(coupon, customer);

        /* Now the coupon should be marked as used */
        Assert.True(await _repository.HasUserUsedCouponAsync(couponCode, customer.Id));
    }
}