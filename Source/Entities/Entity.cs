namespace Comanda.WebApi.Entities;

public abstract class Entity
{
    public int Id { get; set; }
    public bool IsDeleted { get; private set; }
    public void MarkAsDeleted() => IsDeleted = true;
}