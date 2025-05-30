using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entity
{
    public class User : IdentityUser<int>
    {
        public string? Name { get; set; }       // Ekstra profil bilgisi için eklenebilir
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Yeni alan
        public List<Post> Posts { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }

}
