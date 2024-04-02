using Microsoft.AspNetCore.Mvc;
using PustokMVC.Business.Interfaces;

namespace PustokMVC.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IGenreService _genreService;

        public HeaderViewComponent(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await _genreService.GetAllAsync(null);

            return View(genres);
        }
    }
}
