using Core.Domain.Authentication.Models;
using Core.Domain.Utils;

namespace Core.Domain.Toplists.Models
{
    public class AddMovieRequest : AuthorizedModel
    {
        public long ToplistId { get; set; }
        public string MovieImdbId { get; set; }
        public int Position { get; set; }
    }
}