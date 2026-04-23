

namespace SafeBite_Backend_H6.API.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    public BaseRepository(AppDbContext context)
    {
        _context = context;
    }
    public virtual async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        // save from service and set error message there if any
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Set<T>().FindAsync(id);

        if (entity is null)
            return false;

        _context.Set<T>().Remove(entity);
        return true;
    }


    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>()
            .FindAsync(id);
        // find kræver tracking og er optimeret til dette
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        return await _context.Set<T>()
            .FindAsync(id);
        // find kræver tracking og er optimeret til dette
    }

    public virtual void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();

        // da savechanges er en transaction (alt eller intet) vil den fejle hvis 1 af mange changes ikke bliver gemt.
        // der kommer en exception hvis den fejler.
    }


    /*
    https://www.youtube.com/watch?v=F_b5wjkpg8M
    video omkring pagination
    */

    // for at man skal kunne filtrere på resultatet skal vi medtage vores query fra servicen
    // servicen skal ikke stå for at lave selve filteret, det bliver oprettet i et ikke generisk repository
    // servicen må nemlig ikke lave linq queries, da det er repositoryets ansvar at håndtere dataadgangen.
    // derfor kalder vi blot en metode til at få vores query og så bruger vi det til denne metode
    public async Task<PagedResult<T>> GetPagedAsync(PaginationParameters parameters, IQueryable<T>? query = null)
    {
        var source = query ?? _context.Set<T>()
            .AsNoTracking(); // vi laver ikke ændringer så vi behøver ikke at tracke, det gør det hurtigere og mindre ressourcekrævende

        var totalCount = await source.CountAsync();

        var data = await source
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize),
            Data = data
        };
    }

    public async Task<int> CountAsync()
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .CountAsync();
    }

}

