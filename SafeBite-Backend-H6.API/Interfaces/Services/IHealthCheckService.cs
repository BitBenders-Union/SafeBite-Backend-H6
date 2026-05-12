namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface IHealthCheckService
{
    public Task<Boolean> HealthCheckPass();
}
