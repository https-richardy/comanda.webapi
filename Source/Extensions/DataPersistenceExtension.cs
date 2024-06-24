namespace Comanda.WebApi.Extensions;

public static class DataPersistenceExtension
{
    public static void AddDataPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ComandaDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IEstablishementOwnerRepository, EstablishmentOwnerRepository>();
    }
}