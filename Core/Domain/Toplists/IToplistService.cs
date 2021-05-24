using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Data.Models;
using Core.Domain.Toplists.Models;

namespace Core.Domain.Toplists
{
    public interface IToplistService
    {
        Task<IEnumerable<ToplistModel>> BrowseToplists(BrowseToplistsRequest request);
        Task<IEnumerable<ToplistModel>> GetUserToplists(GetToplistsRequest request);
        Task<ToplistModel> GetToplist(GetToplistRequest request);
        Task<ToplistModel> Create(CreateToplistRequest request);
        Task<ToplistModel> Rename(RenameToplistRequest request);
        Task<ToplistModel> AddMovie(AddMovieRequest request);
        Task<ToplistModel> RemoveMovie(RemoveMovieRequest request);
        Task<ToplistModel> ChangePosition(ChangePositionRequest request);
        Task<ToplistModel> ChangePublic(ChangePublicRequest request);
        Task Delete(DeleteToplistRequest request);
    }
}