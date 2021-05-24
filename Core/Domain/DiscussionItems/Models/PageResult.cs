using System.Collections.Generic;

namespace Core.Domain.DiscussionItems.Models
{
    public class PageResult<TItem>
    {
        public IEnumerable<TItem> Items { get; set; }
        public bool HasMore { get; set; }
    }
}