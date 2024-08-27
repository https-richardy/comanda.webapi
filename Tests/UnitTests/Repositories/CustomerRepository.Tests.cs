namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class CustomerRepositoryTests : InMemoryDatabaseFixture<ComandaDbContext>
{
    private readonly ICustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        _repository = new CustomerRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given a valid customer, should update successfully in the database")]
    public async Task GivenValidCustomer_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var customer = Fixture.Create<Customer>();

        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        customer.FullName = "John Doe";

        await _repository.UpdateAsync(customer);
        var updatedCustomer = await DbContext.Customers.FindAsync(customer.Id);

        Assert.NotNull(updatedCustomer);

        Assert.Equal(expected: customer.Id, actual: updatedCustomer.Id);
        Assert.Equal(expected: customer.FullName, actual: updatedCustomer.FullName);
    }

    [Fact(DisplayName = "Given a valid customer, should delete successfully from the database")]
    public async Task GivenValidCustomer_ShouldDeleteSuccessfullyFromTheDatabase()
    {
        var customer = Fixture.Create<Customer>();

        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        await _repository.DeleteAsync(customer);
        var deletedCustomer = await DbContext.Customers.FindAsync(customer.Id);

        Assert.Null(deletedCustomer);
    }
}