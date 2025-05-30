using BlogApp.Entity;

namespace BlogApp.Models
{
    public class UserProfileViewModel
    {
        public string? Name { get; set; }
        public string UserName { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }  // Bu alan User modelinde yok, istersen ekle veya IdentityUser'ın CreatedDate'ini kullan

        public List<UserPostSummary> Posts { get; set; } = new();
        public List<UserCommentSummary> Comments { get; set; } = new();
    }

    public class UserPostSummary
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ContentSnippet { get; set; }
    }

    public class UserCommentSummary
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PostTitle { get; set; }
    }

}
