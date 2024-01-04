﻿using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.Dto
{
    public class ReviewDto
    {
        public ReviewDto() { }
        public ReviewDto(Review review)
        {
            this.Id = review.Id;
            this.AuthorId = review.AuthorId;
            this.Title = review.Title;
            this.LeisureId = review.LeisureId;
            this.Group = review.Group;
            this.AuthorRate = review.AuthorRate;
            this.Tags = review.Tags.Select(t => t.Name).ToList();
            this.LikesCount = review.Likes.Count;
        }

        public string Id { get; set; }

        public string AuthorId { get; set; }

        public string Title { get; set; }

        public string LeisureId { get; set; }

        public LeisureGroup Group { get; set; }

        public byte AuthorRate { get; set; }

        public List<string> Tags { get; set; }

        public int LikesCount { get; set; }
    }
}