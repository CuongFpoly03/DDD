using DomainDrivenDesign.Domain.Entities;

namespace DomainDrivenDesign.Domain.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> FindByEmail(string email);
        Task Register(User user, string password);
        Task<User?> Login(string email, string password);
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(User user);
        Task<(string AccessToken, string RefreshToken)> RefreshToken(string refreshToken); // làm mới token
    }
}