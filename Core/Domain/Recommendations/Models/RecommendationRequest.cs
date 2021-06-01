using Core.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Recommendations.Models
{
    public class RecommendationRequest : AuthorizedModel
    {
        public int RequestNumber { get; set; }
    }
}
