#pragma warning disable CS8604, CS8600

namespace Comanda.WebApi.TestingSuite.Repositories;

public sealed class CustomerRepositoryTestingSuite : IAsyncLifetime
{
    private readonly ComandaDbContext _dbContext;
    private readonly CustomerRepository _repository;
    private readonly IFixture _fixture;

    public CustomerRepositoryTestingSuite()
    {
        var optionsBuilder = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

        _dbContext = new ComandaDbContext(optionsBuilder.Options);
        _repository = new CustomerRepository(_dbContext);

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public async Task InitializeAsync()
    {
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        _dbContext.Dispose();
    }

    [Fact(DisplayName = "SaveAsync - Should save new customer successfully")]
    public async Task SaveAsync_ShouldSaveNewCustomerSuccessfully()
    {
        var customer = _fixture.Create<Customer>();

        await _repository.SaveAsync(customer);

        var savedCustomer = await _dbContext.Customers.FindAsync(customer.Id);

        Assert.NotNull(savedCustomer);
        Assert.Equal(customer.Id, savedCustomer.Id);
        Assert.Equal(customer.FullName, savedCustomer.FullName);

        Assert.Equal(customer.Orders, savedCustomer.Orders);
        Assert.Equal(customer.Addresses, savedCustomer.Addresses);
    }

    [Fact(DisplayName = "SaveAsync - Should throw ArgumentNullException if entity is null")]
    public async Task SaveAsync_ShouldThrowArgumentNullExceptionIfEntityIsNull()
    {
        Customer customer = null;
        Func<Task> act = async () => await _repository.SaveAsync(customer);

        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact(DisplayName = "UpdateAsync - Should update existing customer successfully")]
    public async Task UpdateAsync_ShouldUpdateExistingCustomerSuccessfully()
    {
        var customer = _fixture.Create<Customer>();

        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();

        customer.FullName = "Updated Name";
        await _repository.UpdateAsync(customer);

        var updatedCustomer = await _dbContext.Customers.FindAsync(customer.Id);
        Assert.NotNull(updatedCustomer);
        Assert.Equal("Updated Name", updatedCustomer.FullName);
    }

    [Fact(DisplayName = "UpdateAsync - Should handle scenario where customer to update doesn't exist")]
    public async Task UpdateAsync_ShouldHandleScenarioWhereCustomerToUpdateDoesNotExist()
    {
        var customer = _fixture.Create<Customer>();

        await _repository.UpdateAsync(customer);
        var updatedCustomer = await _dbContext.Customers.FindAsync(customer.Id);

        Assert.Null(updatedCustomer);
    }

    [Fact(DisplayName = "DeleteAsync - Should delete existing customer successfully")]
    public async Task DeleteAsync_ShouldDeleteExistingCustomerSuccessfully()
    {
        var customer = _fixture.Create<Customer>();

        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();

        await _repository.DeleteAsync(customer);

        var deletedCustomer = await _dbContext.Customers.FindAsync(customer.Id);
        Assert.Null(deletedCustomer);
    }

    [Fact(DisplayName = "RetrieveAllAsync - Should retrieve all customers")]
    public async Task RetrieveAllAsync_ShouldRetrieveAllCustomers()
    {
        var customers = _fixture.CreateMany<Customer>(3).ToList();

        await _dbContext.Customers.AddRangeAsync(customers);
        await _dbContext.SaveChangesAsync();

        var retrievedCustomers = await _repository.RetrieveAllAsync();

        Assert.Equal(3, retrievedCustomers.Count());
    }

    [Fact(DisplayName = "CountAsync - Should count all customers")]
    public async Task CountAsync_ShouldCountAllCustomers()
    {
        var customers = _fixture.CreateMany<Customer>(3).ToList();

        await _dbContext.Customers.AddRangeAsync(customers);
        await _dbContext.SaveChangesAsync();

        var count = await _repository.CountAsync();

        Assert.Equal(3, count);
    }

    [Fact(DisplayName = "CountAsync - Should count customers based on specific predicate")]
    public async Task CountAsync_ShouldCountCustomersBasedOnSpecificPredicate()
    {
        // Given
        var customers = _fixture.CreateMany<Customer>(3).ToList();
        customers[0].FullName = "John Doe";

        await _dbContext.Customers.AddRangeAsync(customers);
        await _dbContext.SaveChangesAsync();

        var count = await _repository.CountAsync(customer => customer.FullName == "John Doe");

        Assert.Equal(1, count);
    }

    [Fact(DisplayName = "ExistsAsync - Should check if customer with specific ID exists")]
    public async Task ExistsAsync_ShouldCheckIfCustomerWithSpecificIDExists()
    {
        var customer = _fixture.Create<Customer>();

        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();

        var exists = await _repository.ExistsAsync(customer.Id);

        Assert.True(exists);
    }

    [Fact(DisplayName = "FindAllAsync - Should find all customers based on predicate")]
    public async Task FindAllAsync_ShouldFindAllCustomersBasedOnPredicate()
    {
        var customers = _fixture.CreateMany<Customer>(3).ToList();
        customers[0].FullName = "John Doe";

        await _dbContext.Customers.AddRangeAsync(customers);
        await _dbContext.SaveChangesAsync();

        var foundCustomers = await _repository.FindAllAsync(customer => customer.FullName == "John Doe");

        Assert.Single(foundCustomers);
        Assert.Equal("John Doe", foundCustomers.First().FullName);
    }

    [Fact(DisplayName = "FindSingleAsync - Should find single customer based on predicate that returns one result")]
    public async Task FindSingleAsync_ShouldFindSingleCustomerBasedOnPredicateThatReturnsOneResult()
    {
        var customer = _fixture.Create<Customer>();
        customer.FullName = "John Doe";

        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();

        var foundCustomer = await _repository.FindSingleAsync(c => c.FullName == "John Doe");

        Assert.NotNull(foundCustomer);
        Assert.Equal("John Doe", foundCustomer.FullName);
    }

    [Fact(DisplayName = "FindSingleAsync - Should find single customer based on predicate that returns no results")]
    public async Task FindSingleAsync_ShouldFindSingleCustomerBasedOnPredicateThatReturnsNoResults()
    {
        var customer = _fixture.Create<Customer>();
        customer.FullName = "Jane Doe";

        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();

        var foundCustomer = await _repository.FindSingleAsync(c => c.FullName == "John Doe");

        Assert.Null(foundCustomer);
    }

    [Fact(DisplayName = "PagedAsync - Should retrieve specific page of customers (without predicate)")]
    public async Task PagedAsync_ShouldRetrieveSpecificPageOfCustomersWithoutPredicate()
    {
        var customers = _fixture.CreateMany<Customer>(10).ToList();

        await _dbContext.Customers.AddRangeAsync(customers);
        await _dbContext.SaveChangesAsync();

        var pageNumber = 2;
        var pageSize = 5;
        var pagedCustomers = await _repository.PagedAsync(pageNumber, pageSize);

        Assert.Equal(5, pagedCustomers.Count());
    }

    [Fact(DisplayName = "PagedAsync - Should retrieve specific page of customers based on predicate")]
    public async Task PagedAsync_ShouldRetrieveSpecificPageOfCustomersBasedOnPredicate()
    {
        var customers = _fixture.CreateMany<Customer>(10).ToList();

        customers[0].FullName = "John Doe";
        customers[1].FullName = "John Doe";

        await _dbContext.Customers.AddRangeAsync(customers);
        await _dbContext.SaveChangesAsync();

        var pageNumber = 1;
        var pageSize = 2;
        var pagedCustomers = await _repository.PagedAsync(customer => customer.FullName == "John Doe", pageNumber, pageSize);

        Assert.Equal(2, pagedCustomers.Count());
        Assert.All(pagedCustomers, c => Assert.Equal("John Doe", c.FullName));
    }
}