namespace SafeBite_Backend_H6.API.Services.Allergies;

public class AllergyUserService : IAllergyUserService
{
    private readonly IAllergyUserRepository _repository;
    private readonly IAllergyService _allergyService;
    public AllergyUserService(IAllergyUserRepository repository, IAllergyService allergyService)
    {
        _repository = repository;
        _allergyService = allergyService;
    }

    // create get-all delete is needed

    public async Task<AllergyUserResponse> AddAllergyUserAsync(AllergyUserRequest allergyUserRq, string userId)
    {
        // validation af parameter

        // get allergy
        Allergy allergy = await (_allergyService.GetAllergyByIdAsync(allergyUserRq.AllergyId) ?? throw new Exception($"Allergy with id {allergyUserRq.AllergyId} not found."));

        // mapping

        var allergyUser = AllergyUserMappings.ToEntity(allergyUserRq, allergy, userId);

        // oprettelse
        await _repository.AddAsync(allergyUser);

        // gem
        await _repository.SaveChangesAsync();

        // return
        return AllergyUserMappings.ToResponse(allergyUser);

    }

    public async Task<PagedResult<AllergyUserResponse>> GetAllergyUserPaged(string userId, PaginationParameters parameters, string? searchTerm = null)
    {
        var query = _repository.QueryFilter(userId, searchTerm);

        PagedResult<AllergyUser> result = await _repository.GetPagedAsync(parameters, query);

        PagedResult<AllergyUserResponse> mappedResult = result.Map(AllergyUserMappings.ToResponse);

        return mappedResult;

    }

    public async Task<bool> DeleteAllergyUserAsync(Guid id, string userId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.");

        bool deleted = await _repository.DeleteAsync(id, userId);

        if (deleted)
            await _repository.SaveChangesAsync();

        return deleted;
    }

    public async Task<List<AllergyAnalysisItem>> GetAnalysisItemsByUserIdAsync(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty.");

        var query = _repository.QueryFilter(userId, null);

        var items = await query
            .Select(x => new AllergyAnalysisItem
            {
                AllergyId = x.AllergyId,
                AllergyName = x.Allergy.Name
            })
            .ToListAsync();

        return items;
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _repository.CountAsync();
    }

}
