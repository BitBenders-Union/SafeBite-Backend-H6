namespace SafeBite_Backend_H6.API.Interfaces.Repositories;

public interface ICustomAllergyRepository : IBaseRepository<CustomAllergy>
{
    Task<CustomAllergy?> GetAllergyByNameAsync(string name);
    IQueryable<CustomAllergy> QueryFilter(string? searchTerm);
}
