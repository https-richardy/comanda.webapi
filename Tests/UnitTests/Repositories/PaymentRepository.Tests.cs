namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class PaymentRepositoryTests : SqliteDatabaseFixture<ComandaDbContext>
{
    private readonly IPaymentRepository _repository;

    public PaymentRepositoryTests()
    {
        _repository = new PaymentRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given a new payment, should save successfully in the database")]
    public async Task GivenNewPayment_ShouldSaveSuccessfullyInTheDatabase()
    {
        var order = Fixture.Create<Order>();

        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        var payment = Fixture.Build<Payment>()
            .With(payment => payment.Order, order)
            .Create();

        await _repository.SaveAsync(payment);
        var savedPayment = await DbContext.Payments.FindAsync(payment.Id);

        Assert.NotNull(savedPayment);
        Assert.Equal(payment.Id, savedPayment.Id);
        Assert.Equal(payment.Amount, savedPayment.Amount);
        Assert.Equal(payment.PaymentIntentId, savedPayment.PaymentIntentId);
        Assert.Equal(payment.Order.Id, savedPayment.Order.Id);
    }

    [Fact(DisplayName = "Given an existing payment, should delete it successfully")]
    public async Task GivenExistingPayment_ShouldDeleteSuccessfully()
    {
        var order = Fixture.Create<Order>();

        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        var payment = Fixture.Build<Payment>()
            .With(payment => payment.Order, order)
            .Create();

        await _repository.SaveAsync(payment);
        await _repository.DeleteAsync(payment);

        var deletedPayment = await DbContext.Payments.FindAsync(payment.Id);

        Assert.Null(deletedPayment);
    }

    [Fact(DisplayName = "Given an updated payment, should update it successfully")]
    public async Task GivenUpdatedPayment_ShouldUpdateSuccessfully()
    {
        var order = Fixture.Create<Order>();

        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        var payment = Fixture.Build<Payment>()
            .With(payment => payment.Order, order)
            .Create();

        await _repository.SaveAsync(payment);

        payment.Amount = 100.00m;
        await _repository.UpdateAsync(payment);

        var updatedPayment = await DbContext.Payments.FindAsync(payment.Id);

        Assert.Equal(100.00m, updatedPayment!.Amount);
    }

    [Fact(DisplayName = "Should retrieve a payment by ID successfully")]
    public async Task ShouldRetrievePaymentByIdSuccessfully()
    {
        var order = Fixture.Create<Order>();

        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        var payment = Fixture.Build<Payment>()
            .With(payment => payment.Order, order)
            .Create();

        await _repository.SaveAsync(payment);
        var retrievedPayment = await _repository.RetrieveByIdAsync(payment.Id);

        Assert.NotNull(retrievedPayment);
        Assert.Equal(payment.Id, retrievedPayment.Id);
        Assert.Equal(payment.Amount, retrievedPayment.Amount);
        Assert.Equal(payment.Order.Id, retrievedPayment.Order.Id);
    }

    [Fact(DisplayName = "Should find a payment by order ID")]
    public async Task ShouldFindPaymentByOrderIdSuccessfully()
    {
        var order = Fixture.Create<Order>();

        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        var payment = Fixture.Build<Payment>()
            .With(payment => payment.Order, order)
            .Create();

        await _repository.SaveAsync(payment);
        var foundPayment = await _repository.FindByOrderIdAsync(order.Id);

        Assert.NotNull(foundPayment);
        Assert.Equal(payment.Id, foundPayment.Id);
        Assert.Equal(payment.Order.Id, foundPayment.Order.Id);
    }
}