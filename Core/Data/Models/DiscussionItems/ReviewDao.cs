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

        public static ReviewDao MapReviewToDao(Review review)
        {
            return (ReviewDao)MapDiscussionItemToDao(
                review,
                new ReviewDao()
                {
                    NumberOfStars = review.NumberOfStars
                }
            );
        }

        internal Review MapToReview(long numberOfLikes, long numberOfDislikes, UserDiscussionItemInteractionType? interaction)
        {
            Review copy = new Review()
            {
                NumberOfStars = NumberOfStars
            };

            return (Review)MapToDiscussionItem(copy, numberOfLikes, numberOfDislikes, interaction);
        }
    }
}
