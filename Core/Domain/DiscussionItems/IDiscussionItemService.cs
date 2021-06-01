using Core.Data.Models.DiscussionItems;
using Core.Domain.DiscussionItems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Utils;

namespace Core.Domain.DiscussionItems
{
    public interface IDiscussionItemService
    {
        Task<PageResult<CommentModel>> GetComments(GetCommentsRequest request);
        Task<PageResult<ReviewModel>> GetReviews(GetReviewsRequest request);
        Task<PageResult<FunFactModel>> GetFunFacts(GetFunFactsRequest request);
        Task<ReviewModel> GetUserReview(GetUserReviewRequest request);
        Task<CommentModel> AddComment(AddCommentRequest request);
        Task<ReviewModel> AddReview(AddReviewRequest request);
        Task<FunFactModel> AddFunFact(AddFunFactRequest request);
        Task Interact(InteractRequest request);
        Task Delete(DeleteRequest request);
    }
}
