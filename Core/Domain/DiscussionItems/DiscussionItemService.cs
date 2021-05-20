using Core.Domain.DiscussionItems.Models;
using Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Core.Data.Models.DiscussionItems;

namespace Core.Domain.DiscussionItems
{
    public class DiscussionItemService : IDiscussionItemService
    {
        MovieContext context = new MovieContext();

        public async Task AddComment(Comment comment)
        {
            CommentDao dao = CommentDao.MapCommentToDao(comment);
            await context.AddAsync(dao);
        }

        public async Task AddFunFact(FunFact funFact)
        {
            FunFactDao dao = FunFactDao.MapFunFactToDao(funFact);
            await context.AddAsync(dao);
        }

        public async Task AddReview(Review review)
        {
            ReviewDao dao = ReviewDao.MapReviewToDao(review);
            await context.AddAsync(dao);
        }

        public async Task AddSubcomment(Comment comment)
        {
            await AddComment(comment);
        }

        public async Task<IEnumerable<Comment>> GetCommentsOnDiscussable(long discussableId)
        {
            var commentsDao = await context.Set<CommentDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussableId == discussableId)
                .Take(10)
                .ToListAsync();

            var comments = commentsDao.Select(async c => c.MapToComment(
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Like, c.Id),
                        await GetNumberOfInteractions(UserDiscussionItemInteractionType.Dislike, c.Id),
                        await GetUserInteraction(c.AuthorId, c.Id))).ToList();
            
            await Task.WhenAll(comments);
            return comments.Select(c => c.Result);
        }

        public async Task<IEnumerable<Review>> GetReviewsOnDiscussable(long discussableId)
        {
            var reviewsDao = await context.Set<ReviewDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussableId == discussableId)
                .Take(10)
                .ToListAsync();

            var reviews = reviewsDao.Select(async c => c.MapToReview(
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Like, c.Id),
                        await GetNumberOfInteractions(UserDiscussionItemInteractionType.Dislike, c.Id),
                        await GetUserInteraction(c.AuthorId, c.Id))).ToList();

            await Task.WhenAll(reviews);
            return reviews.Select(c => c.Result);
        }

        public async Task<IEnumerable<FunFact>> GetFunFactsOnDiscussable(long discussableId)
        {
            var funFactsDao = await context.Set<FunFactDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussableId == discussableId)
                .Take(10)
                .ToListAsync();

            var funFacts = funFactsDao.Select(async c => c.MapToFunFact(
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Like, c.Id),
                        await GetNumberOfInteractions(UserDiscussionItemInteractionType.Dislike, c.Id),
                        await GetUserInteraction(c.AuthorId, c.Id))).ToList();

            await Task.WhenAll(funFacts);
            return funFacts.Select(c => c.Result);
        }

        public async Task<IEnumerable<Comment>> GetCommentsOnDiscussionItem(long discussionItemId)
        {
            var commentsDao = await context.Set<CommentDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussionItemId == discussionItemId)
                .Take(10)
                .ToListAsync();

            var comments = commentsDao.Select(async c => c.MapToComment(
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Like, c.Id),
                        await GetNumberOfInteractions(UserDiscussionItemInteractionType.Dislike, c.Id),
                        await GetUserInteraction(c.AuthorId, c.Id))).ToList();

            await Task.WhenAll(comments);
            return comments.Select(c => c.Result);
        }

        private async Task<long> GetNumberOfInteractions(UserDiscussionItemInteractionType interactionType, long discussionItemId)
        {
            return await context.Set<UserDiscussionItemInteraction>()
                .CountAsync(i => i.DiscussionItemId == discussionItemId && i.InteractionType == interactionType);
        }

        private async Task<UserDiscussionItemInteractionType?> GetUserInteraction(long userId, long discussionItemId)
        {
            var userInteraction = await context.Set<UserDiscussionItemInteraction>()
                .SingleOrDefaultAsync(i => i.DiscussionItemId == discussionItemId && i.UserId == userId);
            return userInteraction == null ? null : userInteraction.InteractionType;
        }
    }
}
