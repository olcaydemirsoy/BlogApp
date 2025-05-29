using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BlogApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICommentRepository _commentRepository;

        public PostsController(IPostRepository postRepository, ITagRepository tagRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _commentRepository = commentRepository;
        }

        public IActionResult Index(int? tagId, string? tagText, int page = 1)
        {
            int pageSize = 3;

            var query = _postRepository.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tagText))
            {
                var tagExists = _tagRepository.Tags.Any(t => t.Text == tagText);
                if (tagExists)
                    query = query.Where(p => p.Tags.Any(t => t.Text == tagText));
            }

            int totalPosts = query.Count();

            var posts = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedModel = new PostViewModel
            {
                Posts = posts,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize),
                SelectedTagText = tagText,
                SelectedTagId = tagId
            };

            return View(pagedModel);
        }

        public IActionResult Detail(string title)
        {

            if (string.IsNullOrEmpty(title))
                return RedirectToAction("Index");

            var post = GetSinglePost(title: title);

            if (post == null)
                return RedirectToAction("Index");

            return View(post);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(string commentText, int postId)
        {
            if (string.IsNullOrEmpty(commentText))
                return RedirectToAction("Index");

            // Giriþ yapmýþ kullanýcýnýn ID'sini al
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                // User ID alýnamazsa hata veya yönlendirme yap
                return RedirectToAction("Index");
            }

            var newComment = new Comment
            {
                CommentText = commentText,
                CreatedAt = DateTime.Now,
                UserId = userId,  // Identity user ID
                PostId = postId,
            };

            var post = GetSinglePost(postId); // Senin metot, post'u çekiyor

            _commentRepository.CreateComment(newComment);

            return RedirectToAction("Detail", new { title = post.Title });
        }

        private Post? GetSinglePost(int? postId = null, string? title = null)
        {
            var query = _postRepository.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User);

            if (!string.IsNullOrWhiteSpace(title))
                return query.FirstOrDefault(p => p.Title == title);

            if (postId.HasValue)
                return query.FirstOrDefault(p => p.PostId == postId.Value);

            return null;
        }


    }
}
