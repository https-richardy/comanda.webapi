namespace Comanda.WebApi.Extensions;

public static class DataPersistenceExtension
{
    public static void AddDataPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ComandaDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IAdditionalRepository, AdditionalRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductIngredientRepository, ProductIngredientRepository>();
    }
}