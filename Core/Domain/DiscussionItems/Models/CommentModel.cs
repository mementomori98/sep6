using Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.DiscussionItems.Models
{
    public class CommentModel : DiscussionItemModelBase
    {
        public long? DiscussionItemId { get; set; }
    }
}
