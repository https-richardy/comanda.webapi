namespace Comanda.TestingSuite.Fixtures;

public sealed class WebApiFactoryFixture<TStartup> : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextOptionsDescriptor = services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<ComandaDbContext>));

            if (dbContextOptionsDescriptor is not null)
                services.Remove(dbContextOptionsDescriptor);

            services.AddDbContext<ComandaDbContext>(options =>
            {
                options.UseInMemoryDatabase("Comanda.TestingSuite.Database");
            });

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ComandaDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }
}