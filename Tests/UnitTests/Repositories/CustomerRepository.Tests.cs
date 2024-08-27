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

    [Fact(DisplayName = "Given a valid predicate, should find all matching customers")]
    public async Task GivenValidPredicate_ShouldFindAllMatchingCustomers()
    {
        const string cityToSearch = "Rio de Janeiro";
        var customers = Fixture.CreateMany<Customer>(5).ToList();

        customers[0].Addresses.First().City = cityToSearch;
        customers[1].Addresses.First().City = cityToSearch;

        await DbContext.Customers.AddRangeAsync(customers);
        await DbContext.SaveChangesAsync();

        var foundCustomers = await _repository.FindAllAsync(customer => customer.Addresses.Any(address => address.City == cityToSearch));

        Assert.Equal(2, foundCustomers.Count());

        foreach (var customer in foundCustomers)
        {
            Assert.Contains(customer.Addresses, address => address.City == cityToSearch);
        }
    }

    [Fact(DisplayName = "Given a valid predicate, should find a single customer")]
    public async Task GivenValidPredicate_ShouldFindSingleCustomer()
    {
        var customer = Fixture.Create<Customer>();

        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        var foundCustomer = await _repository.FindSingleAsync(customer => customer.Id == customer.Id);

        Assert.NotNull(foundCustomer);

        Assert.Equal(customer.Id, foundCustomer.Id);
        Assert.Equal(customer.FullName, foundCustomer.FullName);
        Assert.Equal(customer.Account, foundCustomer.Account);
        Assert.Equal(customer.Addresses, foundCustomer.Addresses);
    }

    [Fact(DisplayName = "Should fetch all customers")]
    public async Task ShouldFetchAllCustomers()
    {
        var customers = Fixture.CreateMany<Customer>(5).ToList();

        await DbContext.Customers.AddRangeAsync(customers);
        await DbContext.SaveChangesAsync();

        var foundCustomers = await _repository.RetrieveAllAsync();

        Assert.Equal(customers.Count, foundCustomers.Count());
    }
}