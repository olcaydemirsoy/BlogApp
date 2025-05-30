using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;

        public PostsController(IPostRepository postRepository, ITagRepository tagRepository, ICommentRepository commentRepository, UserManager<User> userManager)
        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _commentRepository = commentRepository;
            _userManager = userManager;
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
                .Where(x=>x.IsActive)
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

            // Giriş yapmış kullanıcının ID'sini al
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                // User ID alınamazsa hata veya yönlendirme yap
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
                .Include(t => t.Tags)
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User);

            if (!string.IsNullOrWhiteSpace(title))
                return query.FirstOrDefault(p => p.Title == title);

            if (postId.HasValue)
                return query.FirstOrDefault(p => p.PostId == postId.Value);

            return null;
        }
        [Authorize]
        [HttpGet]
        public IActionResult CreatePost(int? id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var tags = _tagRepository.Tags.ToList();

            if (id == null)
            {
                // Yeni yazı oluşturma sayfası için ViewModel
                var model = new CreatePostViewModel
                {
                    AvailableTags = tags
                };
                return View(model);
            }
            else
            {
                // Düzenleme sayfası için mevcut yazıyı yükle
                var post = GetSinglePost(id.Value);
                if (post == null)
                    return NotFound();

                var model = new CreatePostViewModel
                {
                    Title = post.Title,
                    Content = post.Content,
                    IsActive = post.IsActive,
                    AvailableTags = tags,
                    SelectedTagIds = post.Tags.Select(t => t.TagId).ToList()
                };

                // Görsel gösterimi için ViewBag'e dosya adını koyuyoruz
                ViewBag.ExistingImageUrl = post.ImageUrl;

                return View(model);
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostViewModel model, int? id)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableTags = _tagRepository.Tags.ToList(); // validasyon hatasında tagları tekrar yükle
                return View(model);
            }

            string? imageFileName = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                Directory.CreateDirectory(uploadsFolder);

                imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, imageFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Index", "Posts");
            }

            var selectedTags = _tagRepository.Tags
                .Where(t => model.SelectedTagIds.Contains(t.TagId))
                .ToList();

            if (id == null)
            {
                // Yeni yazı ekleme
                var newPost = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    ImageUrl = imageFileName,
                    CreatedAt = DateTime.Now,
                    UserId = userId,
                    IsActive = model.IsActive,
                    Tags = selectedTags
                };

                _postRepository.CreatePost(newPost);

                return RedirectToAction("Detail", new { title = newPost.Title });
            }
            else
            {
                // Düzenleme
                var post = GetSinglePost(id.Value);
                if (post == null)
                    return NotFound();

                post.Title = model.Title;
                post.Content = model.Content;
                post.IsActive = model.IsActive;
                post.UserId = userId; // istersen burayı değiştirmeyebilirsin
                post.Tags.Clear();
                post.Tags.AddRange(selectedTags);

                if (imageFileName != null)
                {
                    // Yeni resim yüklendiyse değiştirme
                    post.ImageUrl = imageFileName;
                }
                // Eğer resim yüklenmediyse eski resim kalır.

                _postRepository.UpdatePost(post);

                return RedirectToAction("Detail", new { title = post.Title });
            }
        }


        [Authorize]
        public async Task<IActionResult> MyPosts()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var posts = _postRepository.Posts
                .Where(p => p.UserId == currentUser.Id)
                .Include(p => p.Tags)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(posts);
        }

    }
}
