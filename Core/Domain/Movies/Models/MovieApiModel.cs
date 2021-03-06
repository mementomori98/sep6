using System.Collections.Generic;

namespace Core.Domain.Movies.Models
{
    public class MovieApiModel
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string imdbID { get; set; }
        public string Runtime { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Plot { get; set; }
        public string imdbRating { get; set; }
        public string imdbVotes { get; set; }
        public string Actors { get; set; }
        public IEnumerable<Rating> Ratings { get; set; }
        public long TmdbId { get; set; }
    }
}
