namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class CustomerRepositoryTests : InMemoryDatabaseFixture<ComandaDbContext>
{
    private readonly ICustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        _repository = new CustomerRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given a new customer, should save successfully in the database")]
    public async Task GivenNewCustomer_ShouldSaveSuccessfullyInTheDatabase()
    {
        var customer = Fixture.Create<Customer>();

        await _repository.SaveAsync(customer);
        var savedCustomer = await DbContext.Customers.FindAsync(customer.Id);

        Assert.NotNull(savedCustomer);
        Assert.Equal(customer.Id, savedCustomer.Id);
        Assert.Equal(customer.FullName, savedCustomer.FullName);
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

    [Fact(DisplayName = "Given a valid id, should fetch a customer by id")]
    public async Task GivenValidId_ShouldFetchCustomerById()
    {
        var customer = Fixture.Create<Customer>();

        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        var foundCustomer = await _repository.RetrieveByIdAsync(customer.Id);

        Assert.NotNull(foundCustomer);

        Assert.Equal(customer.Id, foundCustomer.Id);
        Assert.Equal(customer.FullName, foundCustomer.FullName);
        Assert.Equal(customer.Account, foundCustomer.Account);
        Assert.Equal(customer.Addresses, foundCustomer.Addresses);
    }

    [Fact(DisplayName = "Should fetch customers in pages")]
    public async Task ShouldFetchCustomersInPages()
    {
        var customers = Fixture.CreateMany<Customer>(10).ToList();

        await DbContext.Customers.AddRangeAsync(customers);
        await DbContext.SaveChangesAsync();

        const int pageNumber = 1;
        const int pageSize = 5;

        var pagedCustomers = await _repository.PagedAsync(pageNumber, pageSize);
        Assert.Equal(pageSize, pagedCustomers.Count());
    }

    [Fact(DisplayName = "Given a valid userId, should find customer by userId")]
    public async Task GivenValidUserId_ShouldFindCustomerByUserId()
    {
        var user = Fixture.Create<Account>();
        var customer = Fixture.Build<Customer>()
            .With(customer => customer.Account, user)
            .With(customer => customer.FullName, user.UserName)
            .Create();

        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        var foundCustomer = await _repository.FindCustomerByUserIdAsync(user.Id);

        var expectedAddresses = customer.Addresses.ToList();
        var actualAddresses = foundCustomer!.Addresses.ToList();

        Assert.Equal(expectedAddresses.Count, actualAddresses.Count);
        for (int index = 0; index < expectedAddresses.Count; index++)
        {
            var expectedAddress = expectedAddresses[index];
            var actualAddress = actualAddresses[index];

            Assert.Equal(expectedAddress.Street, actualAddress.Street);
            Assert.Equal(expectedAddress.City, actualAddress.City);
            Assert.Equal(expectedAddress.State, actualAddress.State);
            Assert.Equal(expectedAddress.PostalCode, actualAddress.PostalCode);
        }

        var expectedOrders = customer.Orders.ToList();
        var actualOrders = foundCustomer.Orders.ToList();

        Assert.Equal(expectedOrders.Count, actualOrders.Count);
        for (int index = 0; index < expectedOrders.Count; index++)
        {
            var expectedOrder = expectedOrders[index];
            var actualOrder = actualOrders[index];

            Assert.Equal(expectedOrder.Id, actualOrder.Id);
            Assert.Equal(expectedOrder.Date, actualOrder.Date);
        }
    }
}