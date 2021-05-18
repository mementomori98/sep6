using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Movies.Models;

namespace Core.Domain.Movies
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieListModel>> SearchList(string text);
        Task<MovieModel> GetMovieDetails(string ImdbId);
    }
}