using EPR.Domain.Entities;

namespace EPR.Application.Services;

public interface IAuthenticationService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User> CreateUserAsync(string username, string email, string password);
}









