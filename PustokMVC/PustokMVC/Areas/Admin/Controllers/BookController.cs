using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokMVC.Business.Interfaces;
using PustokMVC.CustomExceptions.BookExceptions;
using PustokMVC.Data;
using PustokMVC.Extensions;
using PustokMVC.Helpers;
using PustokMVC.Models;

namespace PustokMVC.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IBookService _bookService;
        private readonly IGenreService _genreService;

        public BookController(PustokDbContext context, 
                            IWebHostEnvironment env, 
                            IBookService bookService, 
                            IGenreService genreService)
        {
            _context = context;
            _env = env;
            _bookService = bookService;
            _genreService = genreService;
        }
        //public async Task<IActionResult> Index()
        //    => View(await _bookService.GetAllAsync(null,"Author","Genre","BookImages"));

        public async Task<IActionResult> Index(int page)
        {
            var datas = _context.Books.AsQueryable();
            datas = datas.Include(x => x.Author).Include(x => x.Genre).Include(x => x.BookImages);

            var paginatedDatas = PaginatedList<Book>.Create(datas, 2, page);

            return View(paginatedDatas);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = await _context.Genres.ToListAsync();
            ViewBag.Authors = await _context.Authors.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            ViewBag.Genres = await _genreService.GetAllAsync();
            ViewBag.Authors = await _context.Authors.ToListAsync();
            if (!ModelState.IsValid) return View();

            try
            {
                await _bookService.CreateAsync(book);
            }
            catch(BookInvalidCredentialException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Genres = await _genreService.GetAllAsync();
            ViewBag.Authors = await _context.Authors.ToListAsync();
            Book? book = null;
            try
            {
                book = await _bookService.GetSingleAsync(x=>x.Id == id, "Author","Genre","BookImages");
            }
            catch (Exception)
            {

                throw;
            }

            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Book book)
        {
            ViewBag.Genres = await _genreService.GetAllAsync();
            ViewBag.Authors = await _context.Authors.ToListAsync();
            if (!ModelState.IsValid) return View();

            Book existData = await _bookService.GetSingleAsync(x => x.Id == book.Id, "Author", "Genre", "BookImages");

            if(book.PosterImageFile is not null)
            {
                if (book.PosterImageFile.ContentType != "image/jpeg" && book.PosterImageFile.ContentType != "image/png")
                {
                    throw new BookInvalidCredentialException("PosterImageFile", "Content type must be png or jpeg!");
                }

                if (book.PosterImageFile.Length > 2097152)
                {
                    throw new BookInvalidCredentialException("PosterImageFile", "Size must be lower than 2mb!");
                }
                FileManager.DeleteFile(_env.WebRootPath, "uploads/books", existData.BookImages.FirstOrDefault(x => x.IsPoster == true)?.ImageUrl);
                if(existData.BookImages.Any(x=>x.IsPoster == true))
                {
                    existData.BookImages.RemoveAll(x => x.IsPoster == true);
                }
                BookImage posterImage = new BookImage()
                {
                    Book = existData,
                    ImageUrl = book.PosterImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    IsPoster = true
                };
                await _context.BookImages.AddAsync(posterImage);
            }
            if(book.HoverImageFile is not null)
            {
                if (book.HoverImageFile.ContentType != "image/jpeg" && book.HoverImageFile.ContentType != "image/png")
                {
                    throw new BookInvalidCredentialException("HoverImageFile", "Content type must be png or jpeg!");
                }

                if (book.HoverImageFile.Length > 2097152)
                {
                    throw new BookInvalidCredentialException("HoverImageFile", "Size must be lower than 2mb!");
                }
                BookImage hoverImage = new BookImage()
                {
                    Book = existData,
                    ImageUrl = book.HoverImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    IsPoster = false
                };
                await _context.BookImages.AddAsync(hoverImage);
            }

            foreach (var imageFile in existData.BookImages.Where(bi => !book.BookImageIds.Contains(bi.Id) && bi.IsPoster == null))
            {
                FileManager.DeleteFile(_env.WebRootPath, "uploads/books", imageFile?.ImageUrl);
            }
            existData.BookImages.RemoveAll(bi => !book.BookImageIds.Contains(bi.Id) && bi.IsPoster == null);

            if(book.ImageFiles is not null)
            {
                foreach (var imageFile in book.ImageFiles)
                {
                    if (imageFile.ContentType != "image/jpeg" && imageFile.ContentType != "image/png")
                    {
                        throw new BookInvalidCredentialException("HoverImageFile", "Content type must be png or jpeg!");
                    }

                    if (imageFile.Length > 2097152)
                    {
                        throw new BookInvalidCredentialException("HoverImageFile", "Size must be lower than 2mb!");
                    }

                    BookImage bookImage = new BookImage()
                    {
                        BookId = book.Id,
                        IsPoster = null,
                        ImageUrl = imageFile.SaveFile(_env.WebRootPath, "uploads/books")
                    };

                    existData.BookImages.Add(bookImage);
                }
            }
            existData.Desc = book.Desc;
            existData.Title = book.Title;
            existData.IsNew = book.IsNew;
            existData.GenreId = book.GenreId;
            existData.BookCode = book.BookCode;
            existData.AuthorId = book.AuthorId;
            existData.CostPrice = book.CostPrice;
            existData.SalePrice = book.SalePrice;
            existData.IsDeleted = book.IsDeleted;
            existData.IsFeatured = book.IsFeatured;
            existData.IsBestSeller = book.IsBestSeller;
            existData.DiscountPercent = book.DiscountPercent;
            existData.ModifiedDate = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
