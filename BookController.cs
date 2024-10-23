using LibraryApp.BLL.DTO;
using LibraryApp.BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;

        public BookController(IBookService bookService, IGenreService genreService, IAuthorService authorService, IWebHostEnvironment webHost)
        {
            _bookService = bookService;
            _genreService = genreService;
            _authorService = authorService;
            _webHost = webHost;
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var books = await _bookService.GetAll(token);

            return View("Index", books);
        }

        public async Task<IActionResult> Create(CancellationToken token)
        {
            var book = new BookDTO
            {
                Authors = await _authorService.GetSelectItem(token),
                Genres = await _genreService.GetSelectItem(token)
            };

            return View("Create", book);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookDTO bookDTO, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                bookDTO.Authors = await _authorService.GetSelectItem(token);
                bookDTO.Genres = await _genreService.GetSelectItem(token);
                return View("Create", bookDTO);
            }

            if(bookDTO.File != null)
            { 
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads");
                string fileName = Path.GetFileName(bookDTO.File.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await bookDTO.File.CopyToAsync(fileStream);
                }

                bookDTO.FileUrl = "/uploads/" + fileName;
            }

            await _bookService.Create(bookDTO, token);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int bookId, CancellationToken token)
        {
            var book = await _bookService.Get(bookId, token);

            if (book == null)
            {
                TempData["Error"] = "Book not found!";
                return RedirectToAction("Index");
            }

            book.Authors = await _authorService.GetSelectItem(token);
            book.Genres = await _genreService.GetSelectItem(token);

            return View("Edit", book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int bookId, BookDTO bookEdit, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Filed to edit book");
                return View("Edit", bookEdit);
            }

            if (bookEdit.File != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads");
                string fileName = Path.GetFileName(bookEdit.File.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await bookEdit.File.CopyToAsync(fileStream);
                }

                bookEdit.FileUrl = "/uploads/" + fileName;
            }

            var book = await _bookService.Get(bookId, token);

            if (book != null)
            {
                book.Title = bookEdit.Title;
                book.Description = bookEdit.Description;
                book.AuthorId = bookEdit.AuthorId;
                book.GenreId = bookEdit.GenreId;
                if (bookEdit.FileUrl != null)
                {
                    book.FileUrl = bookEdit.FileUrl;
                }

                await _bookService.Update(book, token);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Book not found!";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Delete(int bookId, CancellationToken token)
        {
            var author = await _bookService.Get(bookId, token);

            if (author == null)
            {
                TempData["Error"] = "Book not found!";
                return RedirectToAction("Index");
            }

            await _bookService.Delete(author, token);

            return RedirectToAction("Index");
        }
    }
}
