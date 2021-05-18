using Core.Domain.Utils;

namespace Core.Domain.Toplists.Models
{
    public class RemoveMovieRequest : AuthorizedModel
    {
        public long ToplistId { get; set; }
        public long MovieId { get; set; }
    }
}