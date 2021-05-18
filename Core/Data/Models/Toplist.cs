using System.Collections.Generic;

namespace Core.Data.Models
{
    public class Toplist
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UserId { get; set; }
        
        public List<ToplistMovie> ToplistMovies { get; set; }
    }
}