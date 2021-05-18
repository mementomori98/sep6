namespace Core.Domain.Movies.Models
{
    public class Rating
    {
        public string Source { get; set; }
        public string Value { get; set; }
        public long? Votes { get; set; }
    }
}