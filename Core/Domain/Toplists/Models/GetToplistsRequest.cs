using Core.Domain.Authentication.Models;
using Core.Domain.Utils;

namespace Core.Domain.Toplists.Models
{
    public class GetToplistsRequest : AuthorizedModel
    {
        public int Offset { get; set; }
        public int Limit { get; set; } = -1;
    }
}