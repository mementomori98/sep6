using System.Collections.Generic;

namespace Core.Data.Models
{
    public class ToplistDao : DiscussableDao
    {
        public string Name { get; set; }
        public long UserId { get; set; }
        
        public ICollection<ToplistMovieDao> ToplistMovies { get; set; }
    }
}