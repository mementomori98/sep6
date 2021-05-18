using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Models;
using Core.Domain.Authentication;
using Core.Domain.Toplists.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Core.Domain.Toplists
{
    public class ToplistService : IToplistService
    {
        private readonly IAuthenticationService _authenticationService;

        public ToplistService(
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<ToplistModel> Create(CreateToplistRequest request)
        {
            var user = _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized user");
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name must be specified");

            await using var context = new MovieContext();
            var entry = await context.Set<Toplist>().AddAsync(new Toplist
            {
                Name = request.Name,
                UserId = user.Id
            });
            await context.SaveChangesAsync();

            return Map(await Fetch(entry.Entity.Id));
        }

        public async Task<ToplistMovie> AddMovie(AddMovieRequest request)
        {
            var user = _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");
            
            await using var context = new MovieContext();
            var toplist = await Fetch(request.ToplistId);
            
            if (toplist == null)
                throw new ArgumentException("Toplist does not exist");
            if (toplist.UserId != user.Id)
                throw new Exception("User does not own toplist");
            
            var movie =
        }

        public async Task<ToplistMovie> RemoveMovie(RemoveMovieRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ToplistMovie> ChangePosition(ChangePositionRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ToplistModel> Delete(DeleteToplistRequest request)
        {
            throw new NotImplementedException();
        }

        private async Task<Toplist> Fetch(long toplistId)
        {
            await using var context = new MovieContext();
            return await context.Set<Toplist>()
                .Include(tl => tl.ToplistMovies)
                .ThenInclude(tlm => tlm.Movie)
                .SingleOrDefaultAsync(tl => tl.Id == toplistId);
        }

        private ToplistModel Map(Toplist toplist)
        {
            return new ToplistModel
            {
                Id = toplist.Id,
                Name = toplist.Name,
                Items = toplist.ToplistMovies.Select(tlm => new ToplistItem
                {
                    MovieId = tlm.MovieId,
                    Title = tlm.Movie.Title,
                    Position = tlm.Position
                }).OrderBy(i => i.Position)
            };
        }
    }