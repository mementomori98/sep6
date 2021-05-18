using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
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
            return new MovieModel
            {
                ImdbId = model.imdbID,
                Actors = model.Actors.Split(",").Select(s => s.Trim()),
                Director = model.Director,
                Genres = model.Genre.Split(",").Select(s => s.Trim()),
                Plot = model.Plot,
                ImageUrl = model.Poster,
                Runtime = model.Runtime,
                Title = model.Title,
                Type = model.Type,
                Year = int.Parse(model.Year),
                DiscusionItems = new List<DiscussionItem>(),
                Ratings = model.Ratings.Append(new Rating
                {
                    Source = "Imdb",
                    Value = model.imdbRating,
                    Votes = long.Parse(model.imdbVotes.Replace(",", ""))
                })
            };
        }

        public async Task<IEnumerable<MovieListModel>> SearchList(string text)
        {
            var response = await client.GetAsync($"http://www.omdbapi.com/?apikey={ApiKey}&s={text}");
            var content = await response.Content.ReadAsStringAsync();
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            if (!dict.ContainsKey("Search"))
                return new MovieListModel[0];
            var array = dict["Search"];
            new MovieModel
            {
                
            };
            
            return JsonSerializer.Deserialize<IEnumerable<MovieListModel>>(array.ToString());
        }
    }
}