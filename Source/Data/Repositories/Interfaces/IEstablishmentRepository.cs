namespace Comanda.WebApi.Data.Repositories;

public interface IEstablishmentRepository : IRepository<Establishment>
{
    Task AddProductAsync(Establishment establishment, Product product);
    Task AddCategoryAsync(Establishment establishment, Category category);
}