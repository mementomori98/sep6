namespace Core.Data.Models
{
    public class ToplistMovie
    {
        public long ToplistId { get; set; }
        public long MovieId { get; set; }
        public int Position { get; set; }

        public Movie Movie { get; set; }
    }
}