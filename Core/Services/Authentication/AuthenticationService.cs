using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Models;
using Core.Services.Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<AuthToken> Login(LoginRequest request)
        {
            // TODO hash password
            await using var context = new MovieContext();
            var user = await context.Set<User>().SingleOrDefaultAsync(u =>
                u.Username == request.Username &&
                u.PasswordHash == request.Password);

            if (user == null)
                return null;

            var token = Guid.NewGuid().ToString();
            var session = (await context
                .Set<LoginSession>()
                .AddAsync(new LoginSession
                {
                    UserId = user.Id,
                    Token = token,
                    ExpiresAt = request.Persist ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(2)
                })).Entity;

            await context.SaveChangesAsync();

            return new AuthToken
            {
                Value = token,
                Expiry = session.ExpiresAt
            };
        }

        public async Task<bool> IsLoggedIn(AuthToken token)
        {
            await using var context = new MovieContext();
            var value = token?.Value;
            return await context
                .Set<LoginSession>()
                .AnyAsync(ls => ls.Token == value && ls.ExpiresAt > DateTime.UtcNow);
        }

        public async Task Logout(AuthToken token)
        {
            await using var context = new MovieContext();
            var value = token?.Value;
            var session = await context
                .Set<LoginSession>()
                .SingleOrDefaultAsync(ls => ls.Token == value);
            if (session?.ExpiresAt > DateTime.UtcNow)
            {
                session.ExpiresAt = DateTime.UtcNow;
                context.Update(session);
                await context.SaveChangesAsync();
            }
        }

        public async Task<User> GetCurrentUser(AuthToken token)
        {
            await using var context = new MovieContext();
            var s = token?.Value;
            var session = (await context
                .Set<LoginSession>()
                .Include(ls => ls.User)
                .SingleOrDefaultAsync(ls =>
                    ls.Token == s &&
                    ls.ExpiresAt > DateTime.UtcNow));
            return session?.User;
        }

        public async Task<AuthToken> CreateUser(CreateUserRequest request)
        {
            await using var context = new MovieContext();
            var user = await context.Set<User>().SingleOrDefaultAsync(u => u.Username == request.Username);
            if (user != null)
                throw new Exception("Username already exists");
            
            await context.Set<User>()
                .AddAsync(new User
                {
                    Username = request.Username,
                    PasswordHash = request.Password
                });
            await context.SaveChangesAsync();
            
            return await Login(new LoginRequest
            {
                Username = request.Username,
                Password = request.Password
            });
        }
    }
}