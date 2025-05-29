using BlogApp.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concreate.EfCore
{
    public static class SeedData
    {
        public static async Task AddTestDataAsync(IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            if (!context.Tags.Any())
            {
                context.Tags.AddRange(new List<Tag>
                {
                    new Tag { Text = "C#" },
                    new Tag { Text = "ASP.NET Core" },
                    new Tag { Text = "Entity Framework Core" },
                    new Tag { Text = "JavaScript" },
                    new Tag { Text = "HTML" },
                    new Tag { Text = "CSS" }
                });
                await context.SaveChangesAsync();
            }

            if (!userManager.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        UserName = "OlcayDemirsoy",
                        Name = "Olcay Demirsoy",
                        Email = "olcaydemirsoy@gmail.com",
                        ImageUrl = "p1.png"
                    },
                    new User
                    {
                        UserName = "Gokben",
                        Name = "Gokben Demirsoy",
                        Email = "gokbendemirsoy@gmail.com",
                        ImageUrl = "p1.png"
                    }
                };

                foreach (var user in users)
                {
                    var result = await userManager.CreateAsync(user, "394683Aa." +
                        ""); // şifreyi UserManager ile veriyoruz
                    if (!result.Succeeded)
                    {
                        // Hata loglama vs yapabilirsiniz
                        throw new Exception($"Kullanıcı oluşturulamadı: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            if (!context.Posts.Any())
            {
                var user = await userManager.FindByNameAsync("OlcayDemirsoy");
                var tagCSharp = context.Tags.First(t => t.Text == "C#");
                var tagAspNet = context.Tags.First(t => t.Text == "ASP.NET Core");
                var tagEfCore = context.Tags.First(t => t.Text == "Entity Framework Core");

                context.Posts.AddRange(new List<Post>
                {
                    new Post
                    {
                        Title = "C# 10 Features",
                        Content = "C# 10 introduces several new features...",
                        CreatedAt = DateTime.Now,
                        ImageUrl = "1.jpg",
                        UserId = user.Id,
                        IsActive = true,
                        Tags = new List<Tag> { tagCSharp, tagAspNet }
                    },
                    new Post
                    {
                        Title = "ASP.NET Core 6.0",
                        Content = "ASP.NET Core 6.0 is the latest version...",
                        CreatedAt = DateTime.Now,
                        ImageUrl = "2.jpg",
                        UserId = user.Id,
                        IsActive = true,
                        Tags = new List<Tag> { tagAspNet, tagEfCore }
                    },
                    new Post
                    {
                        Title = "Entity Framework Core 6.0",
                        Content = "Entity Framework Core 6.0 is a lightweight...",
                        CreatedAt = DateTime.Now,
                        ImageUrl = "3.jpg",
                        UserId = user.Id,
                        IsActive = true,
                        Tags = new List<Tag> { tagEfCore }
                    },
                    new Post
                    {
                        Title = "JavaScript ES2022 Updates",
                        Content = "ES2022 brings new features to JavaScript...",
                        CreatedAt = DateTime.Now,
                        ImageUrl = "4.jpg",
                        UserId = user.Id,
                        IsActive = true,
                        Tags = new List<Tag> { context.Tags.First(t => t.Text == "JavaScript") }
                    },
                    new Post
                    {
                        Title = "Modern HTML5 Techniques",
                        Content = "HTML5 offers new semantic elements...",
                        CreatedAt = DateTime.Now,
                        ImageUrl = "5.jpg",
                        UserId = user.Id,
                        IsActive = true,
                        Tags = new List<Tag> { context.Tags.First(t => t.Text == "HTML") }
                    },
                    new Post
                    {
                        Title = "CSS Grid and Flexbox",
                        Content = "CSS Grid and Flexbox simplify layout design...",
                        CreatedAt = DateTime.Now,
                        ImageUrl = "6.jpg",
                        UserId = user.Id,
                        IsActive = true,
                        Tags = new List<Tag> { context.Tags.First(t => t.Text == "CSS") }
                    },
                    new Post
                    {
                        Title = "Getting Started with Razor Pages",
                        Content = "Razor Pages is a page-based programming model...",
                        CreatedAt = DateTime.Now,
                        ImageUrl = "7.jpg",
                        UserId = user.Id,
                        IsActive = true,
                        Tags = new List<Tag> { tagAspNet }
                    },
                    new Post
                    {
                        Title = "Dependency Injection in .NET",
                        Content = "Learn how to use dependency injection in .NET...",
                        CreatedAt = DateTime.Now,
                        ImageUrl = "8.jpg",
                        UserId = user.Id,
                        IsActive = true,
                        Tags = new List<Tag> { tagCSharp, tagAspNet }
                    },
                    new Post
                    {
                        Title = "Building REST APIs with ASP.NET Core",
                        Content = "ASP.NET Core makes it easy to build RESTful APIs...",
                        CreatedAt = DateTime.Now,
                        ImageUrl = "9.jpg",
                        UserId = user.Id,
                        IsActive = true,
                        Tags = new List<Tag> { tagAspNet, tagEfCore }
                    }
                });

                await context.SaveChangesAsync();
            }

            if (!context.Comments.Any())
            {
                var user = await userManager.FindByNameAsync("OlcayDemirsoy");

                context.Comments.AddRange(new List<Comment>
                {
                    new Comment
                    {
                        PostId = 1,
                        UserId = user.Id,
                        CommentText = "Great article on C# 10 features!",
                        CreatedAt = DateTime.Now
                    },
                    new Comment
                    {
                        PostId = 2,
                        UserId = user.Id,
                        CommentText = "ASP.NET Core 6.0 is amazing!",
                        CreatedAt = DateTime.Now
                    }
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
