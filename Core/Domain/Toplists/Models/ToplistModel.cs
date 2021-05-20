using System.Collections;
using System.Collections.Generic;

namespace Core.Domain.Toplists.Models
{
    public class ToplistModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public IEnumerable<ToplistItem> Items { get; set; }
    }
}