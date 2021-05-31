using Core.Data;
using Core.Data.Models;
using Core.Domain.Authentication;
using Core.Domain.DiscussionItems;
using Core.Domain.Movies;
using Core.Domain.Movies.Models;
using Core.Domain.Recommendations.Models;
using Core.Domain.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Domain.Recommendations
{
    public class RecommendationsService : IRecommendationService
    {
        private readonly IDiscussionItemService _discussionItemService;
        private readonly IMovieService _movieService;
        private readonly IAuthenticationService _authenticationService;
        private string ApiKey = "d7c20c31c1ae99c2b49085a9c733817e";
        private static HttpClient client = new HttpClient();

        public RecommendationsService(IDiscussionItemService discussionItemService, IAuthenticationService authenticationService, IMovieService movieService)
        {
            _discussionItemService = discussionItemService;
            _authenticationService = authenticationService;
            _movieService = movieService;
        }

        public async Task<List<MovieDao>> GetRecommendationsForUser(RecommendationRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null) return await GetTopRated(GetRecommendationsGetModelForUnauthenticatedUser(request.RequestNumber));
            return await GetRecommendations(await GetRecommendationsGetModel(user.Id, request), user.Id);
        }

        private RecommendationsGetModel GetRecommendationsGetModelForUnauthenticatedUser(int requestNumber)
        {
            return new RecommendationsGetModel() { TopRatedRecommendationPageNumber = requestNumber};
        }
        
        private async Task<RecommendationsGetModel> GetRecommendationsGetModel(long userId, RecommendationRequest request)
        {
            MovieContext context = new MovieContext();
            RecommendationRequestDao dao = await context.Set<RecommendationRequestDao>().SingleOrDefaultAsync(rec => rec.UserId == userId);
            if (dao == null)
            {
                return new RecommendationsGetModel()
                {
                    UserId = userId,
                    Recommendations = new List<ReviewRecommendation>(),
                    RequestPerSession = 1,
                    TopRatedRecommendationPageNumber = 0
                };
            }
            else
            {
                RecommendationsGetModel getModel = dao.MapToGetRecommendationsModel();
                if (getModel.Recommendations == null)
                {
                    getModel.Recommendations = new List<ReviewRecommendation>();
                }
                getModel.RequestPerSession = request.RequestNumber;
                return getModel;
            }
        }

        private async Task<List<MovieDao>> GetRecommendations(RecommendationsGetModel recommendationsGetModel, long userId)
        {
            MovieContext context = new MovieContext();
            if (recommendationsGetModel.RequestPerSession == 1)
            {
                List<ReviewRecommendation> reviews;
                if (recommendationsGetModel.Recommendations.Count() == 0)
                {
                    reviews = await GetReviewRecommendationsExcludingExisting(userId, 3, recommendationsGetModel.Recommendations);
                }
                else
                {
                    reviews = await GetReviewRecommendationsExcludingExisting(userId, 1, recommendationsGetModel.Recommendations);
                }
                recommendationsGetModel.Recommendations = recommendationsGetModel.Recommendations.Concat(reviews).ToList();
            }
            return await GetMoviesFromReviewRecommendations(recommendationsGetModel);
        }

        private async Task<List<MovieDao>> GetMoviesFromReviewRecommendations(RecommendationsGetModel recommendationsGetModel)
        {
            List<MovieDao> recommendations = new List<MovieDao>();
            if (recommendations.Count == 0)
                return await GetTopRated(recommendationsGetModel);
            for(int i = recommendationsGetModel.Recommendations.Count()-1; i > recommendationsGetModel.Recommendations.Count()-4; --i)
            {
                ++recommendationsGetModel.Recommendations[i].NumberOfPagesShown;
                var tmdbIds = await GetSimilarMoviesTmdbIds(recommendationsGetModel.Recommendations[i]);
                
                foreach(TmdbIdModel movieId in tmdbIds)
                {
                    recommendations.Add(await _movieService.GetMovieRecommendation(movieId.id));
                }
            }
            await SaveRecommendationsGetModel(recommendationsGetModel);
            return recommendations;
        }

        public async Task<List<TmdbIdModel>> GetSimilarMoviesTmdbIds(ReviewRecommendation reviewRecommendation)
        {
            var response = await client.GetAsync($"https://api.themoviedb.org/3/movie/{reviewRecommendation.MovieTmdbId}/similar?api_key={ApiKey}&language=en-US&page={reviewRecommendation.NumberOfPagesShown}");
            var content = await response.Content.ReadAsStringAsync();

            var model = JsonSerializer.Deserialize<TmdbSearchResult>(content);
            
            return model.results;
        }

        private async Task<List<ReviewRecommendation>> GetReviewRecommendationsExcludingExisting(long userId, int numberOfEntries, List<ReviewRecommendation> existing)
        {
            var recommendations = await GetReviewRecommendations(userId);
            for(int i = 0; i < recommendations.Count; ++i)
            {
                foreach(ReviewRecommendation recommendation in existing)
                {
                    if(recommendations[i].ReviewId == recommendation.ReviewId)
                    {
                        i--;
                        recommendations.RemoveAt(i+1);
                    }
                }
            }
            return recommendations.Take(numberOfEntries).ToList();
        }

        private async Task<List<ReviewRecommendation>> GetReviewRecommendations(long userId)
        {
            MovieContext context = new MovieContext();
            List<ReviewDao> reviews = context.Set<ReviewDao>()
                    .Where(review => review.AuthorId == userId && review.NumberOfStars == 5)
                    .OrderBy(r => r.Id)
                    .ToList();

            reviews.Concat(context.Set<ReviewDao>()
               .Where(review => review.AuthorId == userId && review.NumberOfStars == 4)
               .OrderBy(r => r.Id)
               .ToList());

            return (await ReviewsToReviewRecommendation(reviews));
        }

        private async Task<List<ReviewRecommendation>> ReviewsToReviewRecommendation(List<ReviewDao> reviews) {
            MovieContext context = new MovieContext();
            List<ReviewRecommendation> toReturn = new List<ReviewRecommendation>();

            for(int i = 0; i < reviews.Count; ++i)
            {
                var movie = await context.Set<MovieDao>().FindAsync(reviews[i].DiscussableId);
                if (movie == null)
                {
                    i -= 1;
                    reviews.RemoveAt(i+1);
                }
                else
                {
                    toReturn.Add(new ReviewRecommendation()
                    {
                        ReviewId = reviews[i].Id,
                        UserId = reviews[i].AuthorId,
                        MovieImdbId = movie.ImdbId,
                        HasMore = true,
                        NumberOfPagesShown = 0,
                        MovieTmdbId = movie.TmdbId
                    });
                }
            }
            return toReturn;
        }

        private async Task<List<MovieDao>> GetTopRated(RecommendationsGetModel recommendationsGetModel)
        {
            var response = await client.GetAsync($"https://api.themoviedb.org/3/movie/top_rated?api_key={ApiKey}&language=en-US&page={recommendationsGetModel.TopRatedRecommendationPageNumber++}");
            var content = await response.Content.ReadAsStringAsync();

            var model = JsonSerializer.Deserialize<TmdbSearchResult>(content);
            if(recommendationsGetModel.UserId != 0)
                await SaveRecommendationsGetModel(recommendationsGetModel);
            return await GetTopRatedMovies(model);
        }

        private async Task<List<MovieDao>> GetTopRatedMovies(TmdbSearchResult model)
        {
            var toReturn = model.results.Select(r => _movieService.GetMovieRecommendation(r.id)).ToList();
            await Task.WhenAll(toReturn);
            return toReturn.Select(x => x.Result).ToList();
        }

        private async Task SaveRecommendationsGetModel(RecommendationsGetModel getModel)
        {
            RecommendationRequestDao dao = new RecommendationRequestDao();
            dao.FromGetRecommendationsModel(getModel);
            MovieContext context = new MovieContext();
            try
            {
                context.Set<RecommendationRequestDao>().Update(dao);
                await context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException e)
            {
                context.Set<RecommendationRequestDao>().Add(dao);
                await context.SaveChangesAsync();
            }
        }
    }
}
