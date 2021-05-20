namespace Core.Data.Models
{
    public class ToplistMovieDao
    {
        public long ToplistId { get; set; }
        public long MovieId { get; set; }
        public int Position { get; set; }

        public MovieDao Movie { get; set; }
    }
}