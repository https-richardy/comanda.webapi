namespace Comanda.TestingSuite.UnitTests.Services;

public sealed class CouponServiceTests
{
    private readonly Mock<ICouponRepository> _couponRepository;
    private readonly Mock<ILogger<CouponService>> _logger;
    private readonly ICouponService _couponService;
    private readonly IFixture _fixture;

    public CouponServiceTests()
    {
        _couponRepository = new Mock<ICouponRepository>();
        _logger = new Mock<ILogger<CouponService>>();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _couponService = new CouponService(
            couponRepository: _couponRepository.Object,
            logger: _logger.Object
        );
    }
    [Fact(DisplayName = "GetCouponByCodeAsync should return coupon when code exists")]
    public async Task GetCouponByCodeAsyncShouldReturnCouponWhenCodeExists()
    {
        const string code = "TESTCOUPONCODE";
        var coupon = _fixture
            .Build<Coupon>()
            .With(coupon => coupon.Code, code)
            .Create();

        _couponRepository
            .Setup(repository => repository.GetCouponByCodeAsync(code))
            .ReturnsAsync(coupon);

        var result = await _couponService.GetCouponByCodeAsync(code);

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
        const string code = "TESTCOUPONCODE";

        _couponRepository
            .Setup(repository => repository.GetCouponByCodeAsync(code))
            .ReturnsAsync((Coupon?)null!);

        var result = await _couponService.GetCouponByCodeAsync(code);

        Assert.Null(result);
    }

    [Fact(DisplayName = "AddCouponAsync should save coupon")]
    public async Task AddCouponAsyncShouldSaveCoupon()
    {
        var coupon = _fixture
            .Build<Coupon>()
            .Create();

        await _couponService.AddCouponAsync(coupon);

        _couponRepository.Verify(repository => repository.SaveAsync(coupon), Times.Once);
    }

    [Fact(DisplayName = "UpdateCouponAsync should update existing coupon")]
    public async Task UpdateCouponAsyncShouldUpdateExistingCoupon()
    {
        var existingCoupon = _fixture.Create<Coupon>();
        var updatedCoupon = _fixture
            .Build<Coupon>()
            .With(coupon => coupon.Id, existingCoupon.Id)
            .Create();

        await _couponService.UpdateCouponAsync(updatedCoupon);

        _couponRepository.Verify(repository => repository.UpdateAsync(updatedCoupon), Times.Once);
    }

    [Fact(DisplayName = "DeleteCouponAsync should delete coupon")]
    public async Task DeleteCouponAsyncShouldDeleteCoupon()
    {
        var coupon = _fixture
            .Build<Coupon>()
            .Create();

        await _couponService.DeleteCouponAsync(coupon);

        _couponRepository.Verify(repository => repository.DeleteAsync(coupon), Times.Once);
    }

    [Fact(DisplayName = "ApplyDiscountAsync should apply discount")]
    public async Task ApplyDiscountAsyncShouldApplyDiscount()
    {
        var coupon = _fixture
            .Build<Coupon>()
            .With(coupon => coupon.Discount, 10m)
            .Create();

        var customer = _fixture
            .Build<Customer>()
            .Create();

        _couponRepository
            .Setup(repository => repository.GetValidCouponAsync(It.Is<string>(code => code == coupon.Code)))
            .ReturnsAsync(coupon);

        const decimal orderValue = 100m;
        var result = await _couponService.ApplyDiscountAsync(
            customer: customer,
            couponCode: coupon.Code,
            value: orderValue
        );

        _couponRepository.Verify(repository => repository.GetValidCouponAsync(coupon.Code), Times.Once);
        _couponRepository.Verify(repository => repository.HasUserUsedCouponAsync(coupon.Code, customer.Id), Times.Once);
        _couponRepository.Verify(repository => repository.AddCouponUsageAsync(coupon, customer), Times.Once);

        Assert.Equal(90m, result);
    }
}