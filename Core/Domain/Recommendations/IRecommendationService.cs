using Core.Data.Models;
using Core.Domain.Movies.Models;
using Core.Domain.Recommendations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Recommendations
{
    public interface IRecommendationService
    {
        Task<List<MovieDao>> GetRecommendationsForUser(RecommendationRequest request);
    }
}
