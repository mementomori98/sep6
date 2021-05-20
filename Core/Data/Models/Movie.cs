using System.Collections.Generic;

namespace Core.Data.Models
{
    public class Movie : DiscussableDao
    {
        public string ImdbId { get; set; }
        public string Title { get; set; }
    }
}