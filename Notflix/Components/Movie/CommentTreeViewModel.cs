using System;
using System.Collections.Generic;
using Core.Domain.DiscussionItems.Models;

namespace Notflix.Components.Movie
{
    public class CommentTreeViewModel
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public string Text { get; set; }
        public string AuthorUsername { get; set; }
        public long Likes { get; set; }
        public long Dislikes { get; set; }
        public Interactions? Interaction { get; set; }
        public IEnumerable<CommentTreeViewModel> Comments { get; set; }
        public bool HasMore { get; set; }
        
        public static CommentTreeViewModel Map(CommentModel c)
        {
            return new CommentTreeViewModel
            {
                Id = c.Id,
                Created = c.Created,
                AuthorUsername = c.AuthorUsername,
                Text = c.Text,
                Likes = c.NumberOfLikes,
                Dislikes = c.NumberOfDislikes,
                Interaction = c.UserInteractionType,
                Comments = new List<CommentTreeViewModel>(),
                HasMore = true // todo add to model
            };
        }
    }
}