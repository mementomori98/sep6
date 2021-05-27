using Core.Domain.DiscussionItems.Models;
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

        public static ReviewDao MapReviewToDao(ReviewModel review)
        {
            return (ReviewDao)MapDiscussionItemToDao(
                review,
                new ReviewDao()
                {
                    NumberOfStars = review.NumberOfStars
                }
            );
        }

        internal ReviewModel MapToReview(long numberOfLikes, long numberOfDislikes, Interactions? interaction)
        {
            ReviewModel copy = new ReviewModel()
            {
                NumberOfStars = NumberOfStars
            };

            return (ReviewModel)MapToDiscussionItem(copy, numberOfLikes, numberOfDislikes, interaction);
        }
    }
}
