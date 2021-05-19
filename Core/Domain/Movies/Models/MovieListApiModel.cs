
using System.Text.Json.Serialization;

namespace Core.Domain.Movies.Models
{
    public class MovieListApiModel
    {
        public string Title { get; set; }
        public string Year { get; set; }
        [JsonPropertyName("imdbID")]
        public string ImdbId { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
    }
}