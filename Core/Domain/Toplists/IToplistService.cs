using System.Threading.Tasks;
using Core.Data.Models;
using Core.Domain.Toplists.Models;

namespace Core.Domain.Toplists
{
    public interface IToplistService
    {
        Task<ToplistModel> Create(CreateToplistRequest request);
        Task<ToplistMovie> AddMovie(AddMovieRequest request);
        Task<ToplistMovie> RemoveMovie(RemoveMovieRequest request);
        Task<ToplistMovie> ChangePosition(ChangePositionRequest request);
        Task<ToplistModel> Delete(DeleteToplistRequest request);
    }
}