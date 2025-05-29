using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.ViewComponents
{
    public class LastPostsViewComponent : ViewComponent
    {
        private readonly IPostRepository _postRepository;

        public LastPostsViewComponent(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            return View(await GetRecentPost());
        }

        public async Task<List<Post>> GetRecentPost() =>await _postRepository.Posts.OrderByDescending(p => p.CreatedAt).Take(3).ToListAsync();
    }
}
