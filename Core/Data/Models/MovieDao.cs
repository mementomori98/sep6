using System.Collections.Generic;

namespace Core.Data.Models
{
    public class MovieDao : DiscussableDao
    {
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Year { get; set; }
    }
}