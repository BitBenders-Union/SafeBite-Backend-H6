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

    public async Task<AllergyUserResponse> AddAllergyUserAsync(AllergyUserRequest allergyUserRq)
    {
        // validation af parameter

        // get allergy
        Allergy allergy = await (_allergyService.GetAllergyByIdAsync(allergyUserRq.AllergyId) ?? throw new Exception($"Allergy with id {allergyUserRq.AllergyId} not found."));

        // mapping

        var allergyUser = AllergyUserMappings.ToEntity(allergyUserRq, allergy);

        // oprettelse
        await _repository.AddAsync(allergyUser);

        // gem
        await _repository.SaveChangesAsync();

        // return
        return AllergyUserMappings.ToResponse(allergyUser);

    }

    public async Task<PagedResult<AllergyUserResponse>> GetAllergyUserPaged(PaginationParameters parameters, string? searchTerm = null)
    {
        var query = _repository.QueryFilter(searchTerm);

        PagedResult<AllergyUser> result = await _repository.GetPagedAsync(parameters, query);

        PagedResult<AllergyUserResponse> mappedResult = result.Map(AllergyUserMappings.ToResponse);

        return mappedResult;

    }

    public async Task<bool> DeleteAllergyUserAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.");

        bool deleted = await _repository.DeleteAsync(id);

        if (deleted)
            await _repository.SaveChangesAsync();

        return deleted;
    }

}
