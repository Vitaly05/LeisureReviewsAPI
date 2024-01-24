﻿using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Models.Dto;
using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LeisureReviewsAPI.Hubs
{
    public class CommentsHub : Hub
    {
        private readonly IUsersRepository usersRepository;

        private readonly IReviewsRepository reviewsRepository;

        private readonly ICommentsRepository commentsRepository;

        public CommentsHub(IUsersRepository usersRepository, IReviewsRepository reviewsRepository, ICommentsRepository commentsRepository)
        {
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
            this.commentsRepository = commentsRepository;
        }

        public async Task Init(string reviewId)
        {
            if (!string.IsNullOrEmpty(reviewId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, reviewId);
                var comments = await commentsRepository.GetCommentsAsync(reviewId);
                await Clients.Caller.SendAsync("init-comments", comments.Select(c => c.ConvertToDto()));
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task Send(string text, string reviewId)
        {
            var comment = await createCommentAsync(text, reviewId);
            await commentsRepository.SaveAsync(comment);
            await Clients.Group(reviewId).SendAsync("new-comment", comment.ConvertToDto());
        }

        private async Task<Comment> createCommentAsync(string text, string reviewId)
        {
            return new Comment()
            {
                Text = text,
                Author = await usersRepository.GetAsync(Context.User),
                Review = await reviewsRepository.GetAsync(reviewId)
            };
        }
    }
}
