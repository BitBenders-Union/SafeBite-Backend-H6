namespace SafeBite_Backend_H6.API.Interfaces.Repositories;

public interface IAllergyRepository : IBaseRepository<Allergy>
{
    Task<Allergy?> GetAllergyByNameAsync(string name);
    IQueryable<Allergy> QueryFilter(string? searchTerm);
}
