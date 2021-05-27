using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Models;
using Core.Domain.DiscussionItems.Models;
using Core.Domain.Movies.Models;

namespace Core.Domain.Movies
{
    public class OmdbMovieService : IMovieService
    {
        private static HttpClient client = new HttpClient();
        private static string ApiKey = "720a2f69";

        public async Task<MovieModel> GetMovieDetails(string ImdbId)
        {
            var response = await client.GetAsync($"http://www.omdbapi.com/?apikey={ApiKey}&i={ImdbId}");
            var content = await response.Content.ReadAsStringAsync();

            // todo add the discussion items

            var model = JsonSerializer.Deserialize<MovieApiModel>(content);
            
            return await Sync(model);
        }

        public async Task<IEnumerable<MovieListModel>> SearchList(string text)
        {
            var response = await client.GetAsync($"http://www.omdbapi.com/?apikey={ApiKey}&s={text}");
            var content = await response.Content.ReadAsStringAsync();
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            if (!dict.ContainsKey("Search"))
                return new MovieListModel[0];
            var array = dict["Search"];
            var list = JsonSerializer.Deserialize<IEnumerable<MovieListApiModel>>(array.ToString());
            return list.Select(m => new MovieListModel
            {
                ImageUrl = m.Poster,
                Title = m.Title,
                ImdbId = m.ImdbId,
                Type = m.Type,
                Year = m.Year
            });
        }

        private async Task<MovieModel> Sync(MovieApiModel model)
        {
            await using var context = new MovieContext();
            var movie = context.Set<MovieDao>()
                .SingleOrDefault(m => m.ImdbId == model.imdbID);
            if (movie == null)
            {
                var entry = await context.Set<MovieDao>().AddAsync(new MovieDao
                {
                    ImdbId = model.imdbID,
                    Title = model.Title
                });
                await context.SaveChangesAsync();
                movie = entry.Entity;
            }
            return new MovieModel
            {
                Id = movie.Id,
                ImdbId = model.imdbID,
                Actors = model.Actors.Split(",").Select(s => s.Trim()),
                Director = model.Director,
                Genres = model.Genre.Split(",").Select(s => s.Trim()),
                Plot = model.Plot,
                ImageUrl = model.Poster,
                Runtime = model.Runtime,
                Title = model.Title,
                Type = model.Type,
                Year = model.Year,
                DiscusionItems = new List<DiscussionItemModelBase>(),
                Ratings = model.Ratings.Append(new Rating
                {
                    Source = "Imdb",
                    Value = model.imdbRating,
                    Votes = long.Parse(model.imdbVotes.Replace(",", ""))
                })
            };
        }
    }
}