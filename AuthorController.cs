using LibraryApp.BLL.DTO;
using LibraryApp.BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    public class AuthorController : Controller
    {
        private IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var authors = await _authorService.GetAll(token);

            return View("Index", authors);
        }

        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(AuthorDTO authorDTO, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", authorDTO);
            }

            await _authorService.Create(authorDTO, token);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int authorId, CancellationToken token)
        {
            var author = await _authorService.Get(authorId, token);

            if (author == null)
            {
                TempData["Error"] = "Author not found!";
                return RedirectToAction("Index");
            }

            return View("Edit", author);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int authorId, AuthorDTO authorEdit, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Filed to edit author");
                return View("Edit", authorEdit);
            }

            var author = await _authorService.Get(authorId, token);

            if (author != null)
            {
                author.FirstName = authorEdit.FirstName;
                author.LastName = authorEdit.LastName;

                await _authorService.Update(author, token);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Author not found!";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Delete(int authorId, CancellationToken token)
        {
            var author = await _authorService.Get(authorId, token);

            if (author == null)
            {
                TempData["Error"] = "Author not found!";
                return RedirectToAction("Index");
            }

            await _authorService.Delete(author, token);

            return RedirectToAction("Index");
        }
    }
}
