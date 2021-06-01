using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Recommendations.Models
{
    class RecommendationsGetModel
    {
        public long UserId { get; set; }
        public int TopRatedRecommendationPageNumber { get; set; }
        public List<ReviewRecommendation> Recommendations { get; set; }
        public int RequestPerSession { get; set; }
    }
}
