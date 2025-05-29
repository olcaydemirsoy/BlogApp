using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concreate.EfCore
{
    public class EfTagRepository : ITagRepository
    {
        private readonly BlogContext _context;
        public EfTagRepository(BlogContext context)
        {
            _context = context;
        }
        public IQueryable<Tag> Tags => _context.Tags.Include(t => t.Posts).ThenInclude(p => p.User);
        public void CreatePost(Tag tags)
        {
            _context.Add(tags);
            _context.SaveChanges();
        }
    }
}
