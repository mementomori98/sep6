using System.Threading.Tasks;
using Core.Data.Models;
using Core.Domain.Authentication.Models;

namespace Core.Domain.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthToken> Login(LoginRequest request);
        Task<bool> IsLoggedIn(AuthToken token);
        Task Logout(AuthToken token);
        Task<UserDao> GetCurrentUser(AuthToken token);
        Task<AuthToken> CreateUser(CreateUserRequest request);
    }
}