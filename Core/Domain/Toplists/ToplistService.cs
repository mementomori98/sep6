using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Models;
using Core.Domain.Authentication;
using Core.Domain.Movies;
using Core.Domain.Toplists.Models;
using Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Core.Domain.Toplists
{
    public class ToplistService : IToplistService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMovieService _movieService;

        public ToplistService(
            IAuthenticationService authenticationService,
            IMovieService movieService)
        {
            _authenticationService = authenticationService;
            _movieService = movieService;
        }

        public async Task<IEnumerable<ToplistModel>> GetToplists(GetToplistsRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized user");
            await using var context = new MovieContext();
            var toplists = await context.Set<ToplistDao>()
                .Include(tl => tl.ToplistMovies).ThenInclude(tlm => tlm.Movie)
                .Where(tl => tl.UserId == user.Id)
                .ToListAsync();
            return toplists.Select(Map);
        }

        public async Task<ToplistModel> GetToplist(GetToplistRequest request)
        {
            await using var context = new MovieContext();
            var toplist = await Fetch(request.ToplistId);
            if (toplist == null)
                throw new ArgumentException("Toplist not found");
            return Map(toplist);
        }

        public async Task<ToplistModel> Create(CreateToplistRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized user");
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name must be specified");

            await using var context = new MovieContext();
            var entry = await context.Set<ToplistDao>().AddAsync(new ToplistDao
            {
                Name = request.Name,
                UserId = user.Id
            });
            await context.SaveChangesAsync();
            
            return Map(await Fetch(entry.Entity.Id));
        }

        public async Task<ToplistModel> Rename(RenameToplistRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized user");
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name must be specified");
            
            await using var context = new MovieContext();
            var toplist = await context.Set<ToplistDao>()
                .SingleOrDefaultAsync(tl => tl.Id == request.ToplistId);

            if (toplist == null)
                throw new ArgumentException("Toplist does not exist");
            if (toplist.UserId != user.Id)
                throw new Exception("User does not own toplist");

            toplist.Name = request.Name;
            context.Update(toplist);
            await context.SaveChangesAsync();
            
            return Map(await Fetch(toplist.Id));
        }

        public async Task<ToplistModel> AddMovie(AddMovieRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");

            await using var context = new MovieContext();
            var toplist = await context.Set<ToplistDao>()
                .Include(tl => tl.ToplistMovies)
                .SingleOrDefaultAsync(tl => tl.Id == request.ToplistId);

            if (toplist == null)
                throw new ArgumentException("Toplist does not exist");
            if (toplist.UserId != user.Id)
                throw new Exception("User does not own toplist");

            if (request.Position < toplist.ToplistMovies.MinFallback(tlm => tlm.Position) ||
                request.Position > toplist.ToplistMovies.MaxFallback(tlm => tlm.Position, -1) + 1)
                throw new ArgumentException($"Invalid position: {request.Position}");

            var movie = await _movieService.GetMovieDetails(request.MovieImdbId);
            if (movie == null)
                throw new Exception("Movie not found");

            foreach (var tlm in toplist.ToplistMovies.Where(x => x.Position >= request.Position))
            {
                tlm.Position += 1;
            }

            toplist.ToplistMovies.Add(new ToplistMovieDao
            {
                ToplistId = toplist.Id,
                MovieId = movie.Id,
                Position = request.Position
            });
            context.Update(toplist);
            await context.SaveChangesAsync();
            return Map(await Fetch(toplist.Id));
        }

        public async Task<ToplistModel> RemoveMovie(RemoveMovieRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");

            await using var context = new MovieContext();
            var toplist = await context.Set<ToplistDao>()
                .Include(tl => tl.ToplistMovies)
                .SingleOrDefaultAsync(tl => tl.Id == request.ToplistId);

            if (toplist == null)
                throw new ArgumentException("Toplist does not exist");
            if (toplist.UserId != user.Id)
                throw new Exception("User does not own toplist");

            var tlm = toplist.ToplistMovies.Single(x => x.MovieId == request.MovieId);
            foreach (var item in toplist.ToplistMovies.Where(x => x.Position > tlm.Position))
                item.Position -= 1;

            toplist.ToplistMovies.Remove(tlm);
            await context.SaveChangesAsync();

            return Map(await Fetch(toplist.Id));
        }

        public async Task<ToplistModel> ChangePosition(ChangePositionRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");

            await using var context = new MovieContext();
            var toplist = await Fetch(request.ToplistId);

            if (toplist == null)
                throw new ArgumentException("Toplist does not exist");
            if (toplist.UserId != user.Id)
                throw new Exception("User does not own toplist");

            var tlm = toplist.ToplistMovies.Single(x => x.MovieId == request.MovieId);
            var tlm2 = toplist.ToplistMovies.Single(x => x.Position == request.Position);
            var pos = tlm.Position;
            context.Update(toplist);
            
            tlm.Position = -1;
            await context.SaveChangesAsync();
            
            tlm2.Position = pos;
            await context.SaveChangesAsync();

            tlm.Position = request.Position;
            await context.SaveChangesAsync();

            return Map(toplist);
        }

        public async Task Delete(DeleteToplistRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");
            
            var toplist = await Fetch(request.ToplistId);
            if (toplist == null)
                throw new ArgumentException("Toplist does not exist");
            if (toplist.UserId != user.Id)
                throw new Exception("Used does not own this toplist");

            await using var context = new MovieContext();
            context.Set<ToplistDao>().Remove(toplist);
            await context.SaveChangesAsync();
        }

        private async Task<ToplistDao> Fetch(long toplistId)
        {
            await using var context = new MovieContext();
            return await context.Set<ToplistDao>()
                .Include(tl => tl.ToplistMovies)
                .ThenInclude(tlm => tlm.Movie)
                .SingleOrDefaultAsync(tl => tl.Id == toplistId);
        }

        private ToplistModel Map(ToplistDao toplist)
        {
            return new ToplistModel
            {
                Id = toplist.Id,
                Name = toplist.Name,
                UserId = toplist.UserId,
                Items = toplist.ToplistMovies.Select(tlm => new ToplistItem
                {
                    MovieId = tlm.MovieId,
                    Title = tlm.Movie.Title,
                    Position = tlm.Position,
                    Year = tlm.Movie.Year,
                    ImageUrl = tlm.Movie.ImageUrl,
                    ImdbId = tlm.Movie.ImdbId
                }).OrderBy(i => i.Position)
            };
        }
    }
}