using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Recommendations.Models
{
    public class ReviewRecommendation
    {
        public long ReviewId { get; set; }
        public long UserId { get; set; }
        public int NumberOfPagesShown { get; set; }
        public bool HasMore { get; set; }
        public string MovieImdbId { get; set; }
        public long MovieTmdbId { get; set; }
    }
}
