using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace BlogApp.Data.Concreate.EfCore
{
    public class EfCommentRepository : ICommentRepository
    {
        private readonly BlogContext _context;
        public EfCommentRepository(BlogContext context)
        {
            _context = context;
        }

        public IQueryable<Comment> Comments => _context.Comments.Include(c => c.User);
        public void CreateComment(Comment comment)
        {
            _context.Add(comment);
            _context.SaveChanges();
        }
    }
}
