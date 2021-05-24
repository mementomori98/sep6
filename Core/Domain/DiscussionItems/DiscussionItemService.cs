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
        private const int PageSize = 10;

        public async Task<long> AddComment(Comment comment)
        {
            CommentDao dao = CommentDao.MapCommentToDao(comment);
            var entry = await context.AddAsync(dao);
            await context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public async Task<long> AddFunFact(FunFact funFact)
        {
            FunFactDao dao = FunFactDao.MapFunFactToDao(funFact);
            var entry = await context.AddAsync(dao);
            await context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public async Task<long> AddReview(Review review)
        {
            ReviewDao dao = ReviewDao.MapReviewToDao(review);
            var entry = await context.AddAsync(dao);
            await context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public async Task<long> AddSubcomment(Comment comment)
        {
            return await AddComment(comment);
        }

        public async Task<PageResult<Comment>> GetCommentsOnDiscussable(long discussableId, int page, long userId)
        {
            var query = context.Set<CommentDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussableId == discussableId)
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * PageSize);
            
            var commentsDao = await query
                .Take(PageSize)
                .ToListAsync();

            return new PageResult<Comment>
            {
                Items = await GetComments(commentsDao, page, userId),
                HasMore = await query.CountAsync() > PageSize
            };
        }

        public async Task<PageResult<Comment>> GetSubcommentsOnDiscussionItem(long discussionItemId, int page, long userId)
        {
            var query = context.Set<CommentDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussionItemId == discussionItemId)
                .OrderByDescending(f => f.Id)
                .Skip((page - 1) * PageSize);
            
            var commentsDao = await query
                .Take(PageSize)
                .ToListAsync();

            return new PageResult<Comment>
            {
                Items = await GetComments(commentsDao, page, userId),
                HasMore = await query.CountAsync() > PageSize
            };
        }

        private async Task<IEnumerable<Comment>> GetComments(List<CommentDao> commentsDao, int page, long userId)
        {
            var comments = commentsDao.Select(async c => await ToComment(c, userId)).ToList();

            await Task.WhenAll(comments);
            return comments.Select(c => c.Result);
        }

        private async Task<Comment> ToComment(CommentDao dao, long userId)
        {
            return dao.MapToComment(
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Like, dao.Id),
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Dislike, dao.Id),
                await GetUserInteraction(userId, dao.Id)
            );
        }

        public async Task<PageResult<Review>> GetReviewsOnDiscussable(long discussableId, int page, long userId)
        {
            var query = context.Set<ReviewDao>()
                .Include(r => r.Author)
                .Where(r => r.DiscussableId == discussableId)
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * PageSize);
            var reviewsDao = await query
                .Take(PageSize)
                .ToListAsync();

            var reviews = reviewsDao.Select(async r => await ToReview(r, userId)).ToList();

            await Task.WhenAll(reviews);
            return new PageResult<Review>
            {
                Items = reviews.Select(c => c.Result),
                HasMore = await query.CountAsync() > PageSize
            };
        }

        private async Task<Review> ToReview(ReviewDao dao, long userId)
        {
            return dao.MapToReview(
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Like, dao.Id),
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Dislike, dao.Id),
                await GetUserInteraction(userId, dao.Id)
            );
        }

        public async Task<PageResult<FunFact>> GetFunFactsOnDiscussable(long discussableId, int page, long userId)
        {
            var query = context.Set<FunFactDao>()
                .Include(f => f.Author)
                .Where(f => f.DiscussableId == discussableId)
                .OrderByDescending(f => f.Id)
                .Skip((page - 1) * PageSize);
            
            var funFactsDao = await query
                .Take(PageSize)
                .ToListAsync();

            var funFacts = funFactsDao.Select(async f => await ToFunFact(f, userId)).ToList();

            await Task.WhenAll(funFacts);
            return new PageResult<FunFact>
            {
                Items = funFacts.Select(c => c.Result),
                HasMore = await query.CountAsync() > PageSize
            };
        }

        private async Task<FunFact> ToFunFact(FunFactDao dao, long userId)
        {
            return dao.MapToFunFact(
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Like, dao.Id),
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Dislike, dao.Id),
                await GetUserInteraction(userId, dao.Id)
            );
        }

        public async Task<UserDiscussionItemInteraction> LikeDiscussionItem(long discussionItemId, long userId)
        {
            return await SaveUserInteractionOnDiscussionItem(discussionItemId, userId, UserDiscussionItemInteractionType.Like);
        }

        public async Task<UserDiscussionItemInteraction> DislikeDiscussionItem(long discussionItemId, long userId)
        {
            return await SaveUserInteractionOnDiscussionItem(discussionItemId, userId, UserDiscussionItemInteractionType.Dislike);
        }

        private async Task<UserDiscussionItemInteraction> SaveUserInteractionOnDiscussionItem(long discussionItemId, long userId, UserDiscussionItemInteractionType type)
        {
            var previousUserInteraction = await context.Set<UserDiscussionItemInteraction>().SingleOrDefaultAsync(i => i.DiscussionItemId == discussionItemId && i.UserId == userId);
            if(previousUserInteraction != null)
                context.Set<UserDiscussionItemInteraction>().Remove(previousUserInteraction);
            
            var entry = await context.AddAsync<UserDiscussionItemInteraction>(
                new UserDiscussionItemInteraction()
                {
                    InteractionType = type,
                    DiscussionItemId = discussionItemId,
                    UserId = userId
                });
            await context.SaveChangesAsync();
            return entry.Entity;
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
