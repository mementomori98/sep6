using Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Movies
{
    public interface IMovieRecommendationService
    {
        Task<MovieDao> GetMovieRecommendation(long tmdbId);
    }
}
