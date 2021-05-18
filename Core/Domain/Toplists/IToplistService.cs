﻿using System.Threading.Tasks;
using Core.Data.Models;
using Core.Domain.Toplists.Models;

namespace Core.Domain.Toplists
{
    public interface IToplistService
    {
        Task<ToplistModel> Create(CreateToplistRequest request);
        Task<ToplistModel> AddMovie(AddMovieRequest request);
        Task<ToplistModel> RemoveMovie(RemoveMovieRequest request);
        Task<ToplistModel> ChangePosition(ChangePositionRequest request);
        Task Delete(DeleteToplistRequest request);
    }
}