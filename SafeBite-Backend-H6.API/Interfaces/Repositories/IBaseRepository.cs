namespace SafeBite_Backend_H6.API.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByIdAsync(string id);
    Task AddAsync(T entity);
    void Update(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<PagedResult<T>> GetPagedAsync(PaginationParameters parameters, IQueryable<T>? query = null);
    Task SaveChangesAsync();
}
