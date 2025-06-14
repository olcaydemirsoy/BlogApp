﻿using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entity
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public List<Tag> Tags { get; set; } = new();

        public List<Comment> Comments { get; set; } = new();

    }
}
