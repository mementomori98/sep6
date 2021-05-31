using Core.Domain.Recommendations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    class RecommendationRequestDao
    {
        public long UserId { get; set; }
        public int TopRatedRecommendationPage { get; set; }
        public List<ReviewRecommendation> Recommendations { get; set; }

        public RecommendationsGetModel MapToGetRecommendationsModel()
        {
            return new RecommendationsGetModel()
            {
                UserId = UserId,
                TopRatedRecommendationPageNumber = TopRatedRecommendationPage,
                Recommendations = Recommendations
            };
        }

        public void FromGetRecommendationsModel(RecommendationsGetModel getModel)
        {
            this.UserId = getModel.UserId;
            this.TopRatedRecommendationPage = getModel.TopRatedRecommendationPageNumber;
            this.Recommendations = getModel.Recommendations;
        }
    }
}
