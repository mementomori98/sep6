using Core.Domain.Movies.Models;
using System.Collections.Generic;

namespace Core.Data.Models
{
    public class MovieDao : DiscussableDao
    {
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Year { get; set; }
        public long TmdbId { get; set; }
        public string Director { get; set; }
        public string Runtime { get; set; }

        public static MovieDao MapFromModel(MovieModel model)
        {
            return new MovieDao()
            {
                ImdbId = model.ImdbId,
                Title = model.Title,
                Year = model.Year,
                ImageUrl = model.ImageUrl,
                TmdbId = model.TmdbId,
                Director = model.Director,
                Runtime = model.Runtime
            };
        }
    }
}