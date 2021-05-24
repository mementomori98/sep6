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

        public async Task<IEnumerable<Comment>> GetCommentsOnDiscussable(long discussableId, int page, long userId)
        {
            var commentsDao = await context.Set<CommentDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussableId == discussableId)
                .OrderByDescending(c => c.Id)
                .Take(page * 10)
                .ToListAsync();

            return await GetComments(commentsDao, page, userId);
        }

        public async Task<IEnumerable<Comment>> GetSubcommentsOnDiscussionItem(long discussionItemId, int page, long userId)
        {
            var commentsDao = await context.Set<CommentDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussionItemId == discussionItemId)
                .OrderByDescending(f => f.Id)
                .Take(page * 10)
                .ToListAsync();

            return await GetComments(commentsDao, page, userId);
        }

        private async Task<IEnumerable<Comment>> GetComments(List<CommentDao> commentsDao, int page, long userId)
        {
            var comments = GetPage(commentsDao, page).Select(async c => await ToComment(c, userId)).ToList();

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

        public async Task<IEnumerable<Review>> GetReviewsOnDiscussable(long discussableId, int page, long userId)
        {
            var reviewsDao = await context.Set<ReviewDao>()
                .Include(r => r.Author)
                .Where(r => r.DiscussableId == discussableId)
                .OrderByDescending(r => r.Id)
                .Take(page * 10)
                .ToListAsync();

            var reviews = GetPage(reviewsDao, page).Select(async r => await ToReview(r, userId)).ToList();

            await Task.WhenAll(reviews);
            return reviews.Select(c => c.Result);
        }

        private async Task<Review> ToReview(ReviewDao dao, long userId)
        {
            return dao.MapToReview(
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Like, dao.Id),
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Dislike, dao.Id),
                await GetUserInteraction(userId, dao.Id)
            );
        }

        public async Task<IEnumerable<FunFact>> GetFunFactsOnDiscussable(long discussableId, int page, long userId)
        {
            var funFactsDao = await context.Set<FunFactDao>()
                .Include(f => f.Author)
                .Where(f => f.DiscussableId == discussableId)
                .OrderByDescending(f => f.Id)
                .Take(page * 10)
                .ToListAsync();

            var funFacts = GetPage(funFactsDao, page).Select(async f => await ToFunFact(f, userId)).ToList();

            await Task.WhenAll(funFacts);
            return funFacts.Select(c => c.Result);
        }

        private async Task<FunFact> ToFunFact(FunFactDao dao, long userId)
        {
            return dao.MapToFunFact(
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Like, dao.Id),
                await GetNumberOfInteractions(UserDiscussionItemInteractionType.Dislike, dao.Id),
                await GetUserInteraction(userId, dao.Id)
            );
        }

        private int GetNumberOfEntriesToReturn(int entriesFromTheDatabase, int page)
        {
            if (entriesFromTheDatabase == page * 10)
                return 10;
            else if (entriesFromTheDatabase > (page - 1) * 10)
                return entriesFromTheDatabase % 10;
            else return 0;
        }

        private IEnumerable<T> GetPage<T>(List<T> list, int page)
        {
            list.Reverse();
            return list.Take(GetNumberOfEntriesToReturn(list.Count, page));
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
