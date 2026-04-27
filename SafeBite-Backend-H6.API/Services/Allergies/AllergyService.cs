namespace SafeBite_Backend_H6.API.Services.Allergies;

public class AllergyService : IAllergyService
{
    private readonly IAllergyRepository _repository;
    public AllergyService(IAllergyRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AllergyResponse>> GetAllergiesPagedAsync(PaginationParameters parameters, string? searchTerm = null)
    {

        var query = _repository.QueryFilter(searchTerm);

        PagedResult<Allergy> result = await _repository.GetPagedAsync(parameters, query);

        PagedResult<AllergyResponse> mappedResult = result.Map(AllergyMappings.ToResponse);
        // ToResponse skal ikke invokes da map tager imod en delegate
        // typen af TResult i PagedResult bliver inferred fra AllergyMappings.ToResponse return type.
        // derfor skal vi ikke selv sætte typen af TResult

        return mappedResult;
    }

    public async Task<AllergyResponse?> GetAllergyByNameAsync(string name)
    {

        ArgumentNullException.ThrowIfNull(name);
        if (name.Trim() == string.Empty)
            throw new ArgumentException("name cannot be empty.");

        Allergy? allergy = await _repository.GetAllergyByNameAsync(name.Trim());

        if (allergy == null)
            return null;

        return AllergyMappings.ToResponse(allergy);

    }

    public async Task<Allergy> GetAllergyByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.");

        Allergy? allergy = await _repository.GetByIdAsync(id);

        if (allergy == null)
            throw new KeyNotFoundException($"Allergy with id {id} not found.");

        return allergy;
    }

    public async Task<AllergyResponse> AddAllergyAsync(AllergyRequest allergyRq)
    {
        // validation
        // null
        ArgumentNullException.ThrowIfNull(allergyRq);

        // empty string
        if (allergyRq.Name.Trim() == string.Empty)
            throw new ArgumentException("Allergy name cannot be empty.");


        Allergy allergy = AllergyMappings.ToEntity(allergyRq);

        await _repository.AddAsync(allergy);
        await _repository.SaveChangesAsync();

        var response = AllergyMappings.ToResponse(allergy);
        return response;
    }

    public async Task UpdateAllergy(AllergyUpdateRequest allergyRq)
    {

        ArgumentNullException.ThrowIfNull(allergyRq);

        // id er nødvendig for update, da vi skal vide hvilken allergi der skal opdateres
        if (allergyRq.Id == Guid.Empty)
            throw new ArgumentException("Id is required for update.");

        var name = allergyRq.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Allergy name cannot be empty.");

        Allergy allergy = await _repository.GetByIdAsync(allergyRq.Id);

        // allergy null check
        if (allergy == null)
            throw new KeyNotFoundException($"Allergy with id {allergyRq.Id} not found.");

        AllergyMappings.ToEntityFromUpdateRequest(allergyRq, allergy);

        _repository.Update(allergy);

        await _repository.SaveChangesAsync();
    }

    public async Task<bool> DeleteAllergyAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.");

        bool deleted = await _repository.DeleteAsync(id);

        if (deleted)
            await _repository.SaveChangesAsync();

        return deleted;
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _repository.CountAsync();
    }


}

