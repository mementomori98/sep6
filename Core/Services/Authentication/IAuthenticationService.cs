using System.Threading.Tasks;
using Core.Data.Models;
using Core.Services.Authentication.Models;

namespace Core.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthToken> Login(LoginRequest request);
        Task<bool> IsLoggedIn(AuthToken token);
        Task Logout(AuthToken token);
        Task<User> GetCurrentUser(AuthToken token);
        Task<AuthToken> CreateUser(CreateUserRequest request);
    }
}