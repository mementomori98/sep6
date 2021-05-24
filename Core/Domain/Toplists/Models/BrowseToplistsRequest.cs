namespace Core.Domain.Toplists.Models
{
    public class BrowseToplistsRequest
    {
        public string Text { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}