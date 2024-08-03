namespace Comanda.WebApi.Data;

public sealed class ComandaDbContext(DbContextOptions options) : IdentityDbContext<Account>(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<UnselectedIngredient> UnselectedIngredients { get; set; }
    public DbSet<ProductIngredient> ProductIngredients { get; set; }
    public DbSet<Additional> Additionals { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var currentAssembly = typeof(ComandaDbContext).Assembly;

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(currentAssembly);
    }
}