using BlogApp.Entity;

namespace BlogApp.Models
{
    public class PostViewModel
    {
        public List<Post> Posts { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? SelectedTagText { get; set; }
        public int? SelectedTagId { get; set; }
    }
}
