using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class DiscussionItemDao
    {
        //comment's/review's/fun fact's id
        public long Id { get; set; }
        public string Text { get; set; }
        public long AuthorId { get; set; }
        //movie/toplist/actor id
        public long? MovieId { get; set; }
        public long? ToplistId { get; set; }
        public long? ActorId { get; set; }
    }
}
