using BlogApp.Entity;

namespace BlogApp.Models
{
    public class TagViewModel
    {
        public List<Tag> Tags { get; set; }
        public string SelectedTagText { get; set; }
        public int SelectedTagId { get; set; }
    }
}
