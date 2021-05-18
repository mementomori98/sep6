using System.Collections.Generic;
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

            //to add the discussion items

            return new MovieModel(JsonSerializer.Deserialize<MovieApiModel>(content));
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