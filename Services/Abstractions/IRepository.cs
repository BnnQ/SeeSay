namespace SeeSay.Services.Abstractions;

public interface IRepository<TEntity>
{
    public Task<ICollection<TEntity>> GetAllAsync();
    public Task<TEntity> GetAsync(int id);
    public Task AddAsync(TEntity entity);
    public Task EditAsync(int id, TEntity editedEntity);
    public Task DeleteAsync(int id);
}