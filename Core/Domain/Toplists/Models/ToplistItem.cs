namespace Core.Domain.Toplists.Models
{
    public class ToplistItem
    {
        public long MovieId { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
    }
}