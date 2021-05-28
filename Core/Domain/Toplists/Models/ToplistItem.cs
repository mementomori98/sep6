namespace Core.Domain.Toplists.Models
{
    public class ToplistItem
    {
        public long MovieId { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
        public string ImageUrl { get; set; }
        public string Year { get; set; }
    }
}