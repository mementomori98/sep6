using System;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Models;
using Core.Domain.Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<AuthToken> Login(LoginRequest request)
        {
            // TODO hash password
            await using var context = new MovieContext();
            var user = await context.Set<UserDao>().SingleOrDefaultAsync(u =>
                u.Username == request.Username &&
                u.PasswordHash == request.Password);

            if (user == null)
                return null;

            var token = Guid.NewGuid().ToString();
            var session = (await context
                .Set<LoginSessionDao>()
                .AddAsync(new LoginSessionDao
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
                .Set<LoginSessionDao>()
                .AnyAsync(ls => ls.Token == value && ls.ExpiresAt > DateTime.UtcNow);
        }

        public async Task Logout(AuthToken token)
        {
            await using var context = new MovieContext();
            var value = token?.Value;
            var session = await context
                .Set<LoginSessionDao>()
                .SingleOrDefaultAsync(ls => ls.Token == value);
            if (session?.ExpiresAt > DateTime.UtcNow)
            {
                session.ExpiresAt = DateTime.UtcNow;
                context.Update(session);
                await context.SaveChangesAsync();
            }
        }

        public async Task<UserDao> GetCurrentUser(AuthToken token)
        {
            await using var context = new MovieContext();
            var s = token?.Value;
            var session = (await context
                .Set<LoginSessionDao>()
                .Include(ls => ls.User)
                .SingleOrDefaultAsync(ls =>
                    ls.Token == s &&
                    ls.ExpiresAt > DateTime.UtcNow));
            return session?.User;
        }

        public async Task<AuthToken> CreateUser(CreateUserRequest request)
        {
            await using var context = new MovieContext();
            var user = await context.Set<UserDao>().SingleOrDefaultAsync(u => u.Username == request.Username);
            if (user != null)
                throw new Exception("Username already exists");
            
            await context.Set<UserDao>()
                .AddAsync(new UserDao
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