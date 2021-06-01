using System.Collections.Generic;
using System.Linq;
using Core.Domain.DiscussionItems.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Movies.Models
{
    public class MovieModel
    {
        public long Id { get; set; }
        public string ImdbId { get; set; }
        public long TmdbId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string Year { get; set; }
        public string Plot { get; set; }
        public string ImageUrl { get; set; }
        public string Runtime { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<string> Actors { get; set; }
        public IEnumerable<DiscussionItemModelBase> DiscusionItems { get; set; }
        public IEnumerable<Rating> Ratings { get; set; }
    }
}



