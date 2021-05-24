using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Models;
using Core.Domain.Authentication;
using Core.Domain.Movies;
using Core.Domain.Toplists.Models;
using Core.Domain.Utils;
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

        public async Task<IEnumerable<ToplistModel>> BrowseToplists(BrowseToplistsRequest request)
        {
            if (request.Offset < 0)
                request.Offset = 0;
            if (request.Limit < 1)
                request.Limit = 10;
            
            await using var context = new MovieContext();
            var query = context.Set<ToplistDao>()
                .Include(tl => tl.ToplistMovies)
                .ThenInclude(tlm => tlm.Movie)
                .Include(tl => tl.User)
                .Where(tl => tl.Public);
            var toplists = await query
                .Where(tl => tl.Name.Contains(request.Text))
                .Skip(request.Offset)
                .Take(request.Limit)
                .ToListAsync();

            return toplists.Select(Map);
        }

        public async Task<IEnumerable<ToplistModel>> GetUserToplists(GetToplistsRequest request)
        {
            var (user, _) = await Validate(request, null);

            await using var context = new MovieContext();
            var toplists = await context.Set<ToplistDao>()
                .Include(tl => tl.ToplistMovies).ThenInclude(tlm => tlm.Movie)
                .Include(tl => tl.User)
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
            var (user, _) = await Validate(request, null);

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name must be specified");

            await using var context = new MovieContext();
            var entry = await context.Set<ToplistDao>().AddAsync(new ToplistDao
            {
                Name = request.Name,
                UserId = user.Id,
                Public = request.Public
            });
            await context.SaveChangesAsync();

            return Map(await Fetch(entry.Entity.Id));
        }

        public async Task<ToplistModel> Rename(RenameToplistRequest request)
        {
            var (_, toplist) = await Validate(request, request.ToplistId);

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name must be specified");

            toplist.Name = request.Name;

            await using var context = new MovieContext();
            context.Update(toplist);
            await context.SaveChangesAsync();

            return Map(await Fetch(toplist.Id));
        }

        public async Task<ToplistModel> AddMovie(AddMovieRequest request)
        {
            var (_, toplist) = await Validate(request, request.ToplistId);


            if (request.Position < toplist.ToplistMovies.MinFallback(tlm => tlm.Position) ||
                request.Position > toplist.ToplistMovies.MaxFallback(tlm => tlm.Position, -1) + 1)
                throw new ArgumentException($"Invalid position: {request.Position}");

            var movie = await _movieService.GetMovieDetails(request.MovieImdbId);
            if (movie == null)
                throw new Exception("Movie not found");

            foreach (var tlm in toplist.ToplistMovies.Where(x => x.Position >= request.Position))
                tlm.Position += 1;

            await using var context = new MovieContext();
            context.Update(toplist);
            
            toplist.ToplistMovies.Add(new ToplistMovieDao
            {
                ToplistId = toplist.Id,
                MovieId = movie.Id,
                Position = request.Position
            });

            await context.SaveChangesAsync();

            return Map(await Fetch(toplist.Id));
        }

        public async Task<ToplistModel> RemoveMovie(RemoveMovieRequest request)
        {
            var (_, toplist) = await Validate(request, request.ToplistId);

            var tlm = toplist.ToplistMovies.Single(x => x.MovieId == request.MovieId);
            
            await using var context = new MovieContext();
            context.Update(toplist);
            
            toplist.ToplistMovies.Remove(tlm);
            await context.SaveChangesAsync();
            
            foreach (var item in toplist.ToplistMovies.Where(x => x.Position > tlm.Position))
                item.Position -= 1;
            
            await context.SaveChangesAsync();

            return Map(await Fetch(toplist.Id));
        }

        public async Task<ToplistModel> ChangePosition(ChangePositionRequest request)
        {
            var (_, toplist) = await Validate(request, request.ToplistId);

            var tlm = toplist.ToplistMovies.Single(x => x.MovieId == request.MovieId);
            var tlm2 = toplist.ToplistMovies.Single(x => x.Position == request.Position);
            var pos = tlm.Position;

            await using var context = new MovieContext();
            context.Update(toplist);

            tlm.Position = -1;
            await context.SaveChangesAsync();

            tlm2.Position = pos;
            await context.SaveChangesAsync();

            tlm.Position = request.Position;
            await context.SaveChangesAsync();

            return Map(toplist);
        }

        public async Task<ToplistModel> ChangePublic(ChangePublicRequest request)
        {
            var (_, toplist) = await Validate(request, request.ToplistId);
            toplist.Public = request.Public;

            await using var context = new MovieContext();
            context.Update(toplist);
            await context.SaveChangesAsync();

            return Map(await Fetch(toplist.Id));
        }

        public async Task Delete(DeleteToplistRequest request)
        {
            var (_, toplist) = await Validate(request, request.ToplistId);

            await using var context = new MovieContext();
            context.Set<ToplistDao>().Remove(toplist);
            await context.SaveChangesAsync();
        }

        private async Task<ToplistDao> Fetch(long toplistId)
        {
            await using var context = new MovieContext();
            var toplist = await context.Set<ToplistDao>()
                .Include(tl => tl.ToplistMovies)
                .ThenInclude(tlm => tlm.Movie)
                .Include(tl => tl.User)
                .SingleOrDefaultAsync(tl => tl.Id == toplistId);
            await context.DisposeAsync();
            return toplist;
        }

        private ToplistModel Map(ToplistDao toplist)
        {
            return new ToplistModel
            {
                Id = toplist.Id,
                Name = toplist.Name,
                UserId = toplist.UserId,
                Username = toplist.User.Username,
                Public = toplist.Public,
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

        private async Task<(UserDao, ToplistDao)> Validate(AuthorizedModel request, long? toplistId)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");

            if (toplistId == null)
                return (user, null);

            var toplist = await Fetch(toplistId.Value);
            if (toplist == null)
                throw new ArgumentException("Toplist does not exist");
            if (toplist.UserId != user.Id)
                throw new Exception("Used does not own this toplist");
            return (user, toplist);
        }
    }
}