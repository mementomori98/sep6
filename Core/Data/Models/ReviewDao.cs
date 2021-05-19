using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class ReviewDao : DiscussionItemDao
    {
        public int NumberOfStars { get; set; }
    }
}
