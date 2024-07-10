namespace Comanda.TestingSuite.RepositoriesTestSuite;

public sealed class CustomerRepositoryTest : InMemoryDatabaseFixture<ComandaDbContext>
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerRepositoryTest()
    {
        _customerRepository = new CustomerRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given a valid customer, should save successfully in the database")]
    public async Task GivenValidCustomer_ShouldSaveSuccessfullyInTheDatabase()
    {
        var customer = Fixture.Create<Customer>();

        await _customerRepository.SaveAsync(customer);
        var savedCustomer = await DbContext.Customers.FindAsync(customer.Id);

        Assert.NotNull(savedCustomer);

        Assert.Equal(customer.Id, savedCustomer.Id);
        Assert.Equal(customer.FullName, savedCustomer.FullName);
        Assert.Equal(customer.Account.Email, savedCustomer.Account.Email);
    }

    [Fact(DisplayName = "Given a valid customer, should update successfully in the database")]
    public async Task GivenValidCustomer_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var customer = Fixture.Create<Customer>();

        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        customer.FullName = "Updated Customer Name";

        await _customerRepository.UpdateAsync(customer);
        var updatedCustomer = await DbContext.Customers.FindAsync(customer.Id);

        Assert.NotNull(updatedCustomer);

        Assert.Equal(customer.Id, updatedCustomer.Id);
        Assert.Equal(customer.FullName, updatedCustomer.FullName);
        Assert.Equal(customer.Account.Email, updatedCustomer.Account.Email);
    }

    [Fact(DisplayName = "Given a valid customer, should delete successfully from the database")]
    public async Task GivenValidCustomer_ShouldDeleteSuccessfullyFromTheDatabase()
    {
        var customer = Fixture.Create<Customer>();

        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        await _customerRepository.DeleteAsync(customer);
        var deletedCustomer = await DbContext.Customers.FindAsync(customer.Id);

        Assert.Null(deletedCustomer);
    }

    #pragma warning disable CS8602
    [Fact(DisplayName = "Given a valid predicate, should find all matching customers")]
    public async Task GivenValidPredicate_ShouldFindAllMatchingCustomers()
    {
        var customers = Fixture.CreateMany<Customer>(5).ToList();

        customers[0].FullName = "John Doe";
        customers[1].FullName = "Jane Doe";
        customers[2].FullName = "John Smith";

        await DbContext.Customers.AddRangeAsync(customers);
        await DbContext.SaveChangesAsync();

        var foundCustomers = await _customerRepository.FindAllAsync(customer => customer.FullName.Contains("Doe"));

        Assert.Equal(2, foundCustomers.Count());
    }

    [Fact(DisplayName = "Given a valid predicate, should find a single customer")]
    public async Task GivenValidPredicate_ShouldFindSingleCustomer()
    {
        var customer = Fixture.Create<Customer>();

        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        var foundCustomer = await _customerRepository.FindSingleAsync(c => c.Id == customer.Id);

        Assert.NotNull(foundCustomer);

        Assert.Equal(customer.Id, foundCustomer.Id);
        Assert.Equal(customer.FullName, foundCustomer.FullName);
        Assert.Equal(customer.Account.Email, foundCustomer.Account.Email);
    }

    [Fact(DisplayName = "Should fetch all customers")]
    public async Task ShouldFetchAllCustomers()
    {
        var customers = Fixture.CreateMany<Customer>(5).ToList();

        await DbContext.Customers.AddRangeAsync(customers);
        await DbContext.SaveChangesAsync();

        var foundCustomers = await _customerRepository.RetrieveAllAsync();

        Assert.Equal(customers.Count, foundCustomers.Count());
    }

    [Fact(DisplayName = "Given a valid id, should fetch a customer by id")]
    public async Task GivenValidId_ShouldFetchCustomerById()
    {
        var customer = Fixture.Create<Customer>();

        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        var foundCustomer = await _customerRepository.RetrieveByIdAsync(customer.Id);

        Assert.NotNull(foundCustomer);
        Assert.Equal(customer.Id, foundCustomer.Id);
        Assert.Equal(customer.FullName, foundCustomer.FullName);
        Assert.Equal(customer.Account.Email, foundCustomer.Account.Email);
    }

    [Fact(DisplayName = "Should fetch customers in pages")]
    public async Task ShouldFetchCustomersInPages()
    {
        var customers = Fixture.CreateMany<Customer>(10).ToList();

        await DbContext.Customers.AddRangeAsync(customers);
        await DbContext.SaveChangesAsync();

        var pageNumber = 1;
        var pageSize = 5;

        var pagedCustomers = await _customerRepository.PagedAsync(pageNumber, pageSize);

        Assert.Equal(pageSize, pagedCustomers.Count());
    }

    [Fact(DisplayName = "Given a valid predicate, should fetch customers in pages")]
    public async Task GivenValidPredicate_ShouldFetchCustomersInPages()
    {
        var customers = Fixture.CreateMany<Customer>(10).ToList();

        customers[0].FullName = "John Doe";
        customers[1].FullName = "Jane Doe";
        customers[2].FullName = "John Smith";

        await DbContext.Customers.AddRangeAsync(customers);
        await DbContext.SaveChangesAsync();

        var pageNumber = 1;
        var pageSize = 5;

        var pagedCustomers = await _customerRepository.PagedAsync(customer => customer.FullName.Contains("Doe"), pageNumber, pageSize);

        Assert.Equal(2, pagedCustomers.Count());
    }
}