using Core.Domain.DiscussionItems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data.Models.DiscussionItems;

namespace Core.Data.Models
{
    public class DiscussionItemDao
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public string Text { get; set; }
        public long AuthorId { get; set; }
        public long? DiscussableId { get; set; }
        public UserDao Author { get; set; }
        
        public ICollection<InteractionDao> Interactions { get; set; }
        public ICollection<CommentDao> Comments { get; set; }
    }
}
