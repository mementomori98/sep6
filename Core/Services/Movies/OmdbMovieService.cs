using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Services.Movies.Models;

namespace Core.Services.Movies
{
    public class OmdbMovieService : IMovieService
    {
        private static HttpClient client = new HttpClient();
        private static string ApiKey = "720a2f69";

        public async Task<MovieModal> GetMovieDetails(string ImdbId)
        {
            var response = await client.GetAsync($"http://www.omdbapi.com/?apikey={ApiKey}&i={ImdbId}");
            var content = await response.Content.ReadAsStringAsync();

            //to add the discussion items
            
            return new MovieModal(JsonSerializer.Deserialize<MovieApiModal>(content));
        }

        public async Task<IEnumerable<MovieListModal>> SearchList(string text)
        {
            var response = await client.GetAsync($"http://www.omdbapi.com/?apikey={ApiKey}&s={text}");
            var content = await response.Content.ReadAsStringAsync();
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            if (!dict.ContainsKey("Search"))
                return new MovieListModal[0];
            var array = dict["Search"];
            return JsonSerializer.Deserialize<IEnumerable<MovieListModal>>(array.ToString());
        }
    }
}