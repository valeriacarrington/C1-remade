using LibraryApp.BLL.DTO;
using LibraryApp.BLL.IService;
using LibraryApp.BLL.Service;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    public class GenreController : Controller
    {
        private IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var genres = await _genreService.GetAll(token);

            return View("Index", genres);
        }

        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(GenreDTO genreDTO, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", genreDTO);
            }

            await _genreService.Create(genreDTO, token);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int genreId, CancellationToken token)
        {
            var genre = await _genreService.Get(genreId, token);

            if (genre == null)
            {
                TempData["Error"] = "Genre not found!";
                return RedirectToAction("Index");
            }

            return View("Edit", genre);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int genreId, GenreDTO genreEdit, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Filed to edit genre");
                return View("Edit", genreEdit);
            }

            var genre = await _genreService.Get(genreId, token);

            if (genre != null)
            {
                genre.Name = genreEdit.Name;

                await _genreService.Update(genre, token);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Genre not found!";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Delete(int genreId, CancellationToken token)
        {
            var genre = await _genreService.Get(genreId, token);

            if (genre == null)
            {
                TempData["Error"] = "Genre not found!";
                return RedirectToAction("Index");
            }

            await _genreService.Delete(genre, token);

            return RedirectToAction("Index");
        }
    }
}
