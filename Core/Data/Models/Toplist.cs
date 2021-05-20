using System.Collections.Generic;

namespace Core.Data.Models
{
    public class Toplist : DiscussableDao
    {
        public string Name { get; set; }
        public long UserId { get; set; }
        
        public ICollection<ToplistMovie> ToplistMovies { get; set; }
    }
}