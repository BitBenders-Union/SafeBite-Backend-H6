namespace SafeBite_Backend_H6.API.Services.Allergies;

public class CustomAllergyService : ICustomAllergyService
{
    private readonly ICustomAllergyRepository _repository;

    public CustomAllergyService(ICustomAllergyRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CustomAllergyResponse>> GetCustomAllergiesPagedAsync(string userId, PaginationParameters parameters, string? searchTerm = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty or whitespace.", nameof(userId));

        var query = _repository.QueryFilter(userId, searchTerm);

        PagedResult<CustomAllergy> result = await _repository.GetPagedAsync(parameters, query);

        return result.Map(CustomAllergyMappings.ToResponse);
    }

    public async Task<CustomAllergyResponse> AddCustomAllergyAsync(CustomAllergyRequest customAllergyRequest, string userId)
    {
        ArgumentNullException.ThrowIfNull(customAllergyRequest);

        if (string.IsNullOrWhiteSpace(customAllergyRequest.Name))
            throw new ArgumentException("Name cannot be empty or whitespace.", nameof(customAllergyRequest.Name));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty or whitespace.", nameof(userId));

        CustomAllergy customAllergy = CustomAllergyMappings.ToEntity(customAllergyRequest, userId);

        await _repository.AddAsync(customAllergy);
        await _repository.SaveChangesAsync();

        return CustomAllergyMappings.ToResponse(customAllergy);
    }

    public async Task UpdateCustomAllergyAsync(CustomAllergyUpdateRequest customAllergyUpdateRequest, string userId)
    {
        ArgumentNullException.ThrowIfNull(customAllergyUpdateRequest);

        if (customAllergyUpdateRequest.Id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(customAllergyUpdateRequest.Id));

        if (string.IsNullOrWhiteSpace(customAllergyUpdateRequest.Name))
            throw new ArgumentException("Name cannot be empty or whitespace.", nameof(customAllergyUpdateRequest.Name));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty or whitespace.", nameof(userId));

        CustomAllergy existingCustomAllergy = await GetCustomAllergyByIdAsync(customAllergyUpdateRequest.Id, userId);

        CustomAllergyMappings.ToEntityFromUpdateRequest(customAllergyUpdateRequest, existingCustomAllergy);

        _repository.Update(existingCustomAllergy);
        await _repository.SaveChangesAsync();
    }

    public async Task<bool> DeleteAllergyAsync(Guid customAllergyId, string userId)
    {
        if (customAllergyId == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(customAllergyId));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty or whitespace.", nameof(userId));

        bool deleted = await _repository.DeleteAsync(customAllergyId, userId);

        if (deleted)
            await _repository.SaveChangesAsync();

        return deleted;
    }

    private async Task<CustomAllergy> GetCustomAllergyByIdAsync(Guid customAllergyId, string userId)
    {
        if (customAllergyId == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(customAllergyId));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty or whitespace.", nameof(userId));

        CustomAllergy? customAllergy = await _repository.GetByIdAsync(customAllergyId, userId);

        if (customAllergy is null)
            throw new KeyNotFoundException($"Custom allergy with id {customAllergyId} not found.");

        return customAllergy;
    }

    public async Task<List<AllergyAnalysisItem>> GetAnalysisItemsByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty or whitespace.", nameof(userId));

        var query = _repository.QueryFilter(userId, null);

        return await query
            .Select(x => new AllergyAnalysisItem
            {
                AllergyId = x.Id,
                AllergyName = x.Name
            })
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _repository.CountAsync();
    }
}
