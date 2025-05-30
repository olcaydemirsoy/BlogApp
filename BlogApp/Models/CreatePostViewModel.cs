using BlogApp.Entity;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class CreatePostViewModel
    {
        public int PostId { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public IFormFile? ImageFile { get; set; }

        // Tag seçimi için
        public List<int> SelectedTagIds { get; set; } = new List<int>();

        public List<Tag>? AvailableTags { get; set; }
        public bool IsActive { get; set; } = true;
    }

}