namespace SafeBite_Backend_H6.API.Services.Allergies;

public class CustomAllergyService : ICustomAllergyService
{
    private readonly ICustomAllergyRepository _repository;

    public CustomAllergyService(ICustomAllergyRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CustomAllergyResponse>> GetCustomAllergiesPagedAsync(PaginationParameters parameters, string? searchTerm = null)
    {
        var query = _repository.QueryFilter(searchTerm);

        PagedResult<CustomAllergy> result = await _repository.GetPagedAsync(parameters, query);

        PagedResult<CustomAllergyResponse> pagedResult = result.Map(CustomAllergyMappings.ToResponse);

        return pagedResult;
    }

    public async Task<CustomAllergyResponse> AddCustomAllergyAsync(CustomAllergyRequest customAllergyRequest)
    {

        ArgumentNullException.ThrowIfNull(customAllergyRequest);

        if (customAllergyRequest.Name.Trim() == string.Empty)
            throw new ArgumentException("Name cannot be empty or whitespace.", nameof(customAllergyRequest.Name));

        if (customAllergyRequest.UserId.Trim() == string.Empty)
            throw new ArgumentException("UserId cannot be empty or whitespace.", nameof(customAllergyRequest.UserId));

        CustomAllergy customAllergy = CustomAllergyMappings.ToEntity(customAllergyRequest);

        await _repository.AddAsync(customAllergy);
        await _repository.SaveChangesAsync();

        var response = CustomAllergyMappings.ToResponse(customAllergy);

        return response;
    }

    public async Task UpdateCustomAllergyAsync(CustomAllergyUpdateRequest customAllergyUpdateRequest)
    {
        ArgumentNullException.ThrowIfNull(customAllergyUpdateRequest);

        if (customAllergyUpdateRequest.Id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(customAllergyUpdateRequest.Id));

        if (customAllergyUpdateRequest.Name.Trim() == string.Empty)
            throw new ArgumentException("Name cannot be empty or whitespace.", nameof(customAllergyUpdateRequest.Name));

        if (customAllergyUpdateRequest.UserId.Trim() == string.Empty)
            throw new ArgumentException("UserId cannot be empty or whitespace.", nameof(customAllergyUpdateRequest.UserId));


        CustomAllergy existingCustomAllergy = await GetCustomAllergyByIdAsync(customAllergyUpdateRequest.Id);

        if (existingCustomAllergy == null)
            throw new KeyNotFoundException($"Custom allergy with id {customAllergyUpdateRequest.Id} not found.");

        CustomAllergyMappings.ToEntityFromUpdateRequest(customAllergyUpdateRequest, existingCustomAllergy);

        _repository.Update(existingCustomAllergy);
        await _repository.SaveChangesAsync();
    }

    public async Task<bool> DeleteAllergyAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));

        if (await GetCustomAllergyByIdAsync(id) is null)
            throw new KeyNotFoundException($"Custom allergy with id {id} not found.");

        bool deleted = await _repository.DeleteAsync(id);

        if (deleted)
            await _repository.SaveChangesAsync();

        return deleted;
    }


    private async Task<CustomAllergy> GetCustomAllergyByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));

        CustomAllergy? customAllergy = await _repository.GetByIdAsync(id);

        if (customAllergy == null)
            throw new KeyNotFoundException($"Custom allergy with id {id} not found.");

        return customAllergy;
    }
}
