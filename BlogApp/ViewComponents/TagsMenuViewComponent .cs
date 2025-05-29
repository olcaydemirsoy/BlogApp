using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.ViewComponents
{
    public class TagsMenuViewComponent : ViewComponent
    {
        private readonly ITagRepository _tagRepository;

        public TagsMenuViewComponent(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            return View(await GetTagsAsync());
        }

        public Task<TagViewModel> GetTagsAsync()
        {
            return Task.FromResult(new TagViewModel()
            {
                Tags = _tagRepository.Tags.ToList(),
                SelectedTagText = HttpContext.Request.Query.ContainsKey("tagText") ? string.Format(HttpContext.Request.Query["tagText"]!) : string.Empty,
                SelectedTagId = HttpContext.Request.Query.ContainsKey("tagId") ? int.Parse(HttpContext.Request.Query["tagId"]!) : 0
                
            });
        }
    }
}
