using System.Security.Claims;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BlogApp.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Giriş Sayfası
        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // Giriş İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Email ile kullanıcıyı bul
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
                return View(model);
            }

            // Şifreyi doğrula ve giriş yap
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Hesabınız kilitlenmiş.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
            return View(model);
        }

        // Çıkış
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Kayıt Sayfası
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kullanıcı adı ve e-posta kontrolü (opsiyonel, UserManager da kontrol yapar)
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Bu e-posta zaten kayıtlı.");
                return View(model);
            }

            var user = new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                Name = model.Name,
                ImageUrl = "p1.png"// Eğer ApplicationUser'da varsa
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // İstersen otomatik giriş yapabilirsin
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [Authorize]
        public IActionResult Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Index", "Home");

            var user = _userManager.Users
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .ThenInclude(c => c.Post)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound();

            var model = new UserProfileViewModel
            {
                Name = user.Name,
                UserName = user.UserName,
                ImageUrl = user.ImageUrl,
                CreatedAt = user.CreatedAt, // User modeline eklediysen
                Posts = user.Posts.Select(p => new UserPostSummary
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    CreatedAt = p.CreatedAt,
                    ContentSnippet = p.Content.Length > 100 ? p.Content.Substring(0, 100) + "..." : p.Content
                }).OrderByDescending(p => p.CreatedAt).ToList(),

                Comments = user.Comments.Select(c => new UserCommentSummary
                {
                    CommentId = c.CommentId,
                    CommentText = c.CommentText,
                    CreatedAt = c.CreatedAt,
                    PostTitle = c.Post.Title
                }).OrderByDescending(c => c.CreatedAt).ToList()
            };

            return View(model);
        }

    }
}
