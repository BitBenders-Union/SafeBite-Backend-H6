namespace SafeBite_Backend_H6.API.Interfaces.Services.Users;

public interface IUserService
{
    Task<int> GetTotalActiveUserCount();
    Task<int> GetTotalInactiveUserCount();
    Task<int> GetTotalUserCount();
}
