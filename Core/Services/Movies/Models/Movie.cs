using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Movies.Models
{
    public class Movie
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string ImdbId { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Plot { get; set; }
        public string ImdbRating { get; set; }
        public string ImdbVotes { get; set; }
        public List<string> Actors { get; set; }
        public List<DiscussionItem> DiscusionItems { get; set; }

        public Movie(MovieApiModel apiModel)
        {
            this.Title = apiModel.Title;
            this.Year = apiModel.Year;
            this.ImdbId = apiModel.imdbID;
            this.Type = apiModel.Type;
            this.Poster = apiModel.Poster;
            this.Genre = apiModel.Genre;
            this.Director = apiModel.Director;
            this.Plot = apiModel.Plot;
            this.ImdbRating = apiModel.imdbRating;
            this.ImdbVotes = apiModel.imdbVotes;
            this.Actors = apiModel.Actors.Split(", ").ToList();
        }
    }
}
