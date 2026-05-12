namespace SafeBite_Backend_H6.API.Services;

public class HealthCheckService : IHealthCheckService
{
    private readonly IAllergyRepository _allergyRepository;

    public HealthCheckService(IAllergyRepository allergyRepository)
    {
        _allergyRepository = allergyRepository;
    }

    //This is hardcoded because it's only used for API health check.
    public async Task<bool> HealthCheckPass()
    {
        var response = await _allergyRepository.GetAllergyByNameAsync("Gluten"); //Hardcoded seeded allergy to check if we have connection to DB too.

        if (response == null)
            return false;

        return true;
    }
}
