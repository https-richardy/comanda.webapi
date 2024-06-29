namespace Comanda.WebApi.Data;

public sealed class ComandaDbContext(DbContextOptions options) : IdentityDbContext<Account>(options)
{
    public DbSet<Establishment> Establishments { get; set; }
    public DbSet<EstablishmentOwner> EstablishmentOwners { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Address> Addresses { get; set; }
}