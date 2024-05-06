using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokMVC.Business.Interfaces;
using PustokMVC.Data;
using PustokMVC.Models;
using PustokMVC.ViewModels;

namespace PustokMVC.Controllers
{
    public class ShopController : Controller
    {
        private readonly IBookService _bookService;
        private readonly PustokDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ShopController(
                IBookService bookService,
                PustokDbContext context,
                UserManager<AppUser> userManager)
        {
            _bookService = bookService;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id) 
        {
            var book = await _bookService.GetSingleAsync(x=>x.Id == id,"BookImages","Genre","Author");
            return View(book);
        }
        public async Task<IActionResult> AddToBasket(int bookId)
        {
            if(!await _context.Books.AnyAsync(x=>x.Id == bookId)) return NotFound(); // 404

            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            BasketItemViewModel basketItem = null;
            BasketItem userBasketItem = null;
            var basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

            AppUser appUser = null;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //appUser = await _context.Users.FirstOrDefaultAsync(x=>x.NormalizedUserName == HttpContext.User.Identity.Name.ToUpper());
            }

            if(appUser is null)
            {
                if (basketItemsStr is not null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                    basketItem = basketItems.FirstOrDefault(x => x.BookId == bookId);

                    if (basketItem is not null)
                    {
                        basketItem.Count++;
                    }
                    else
                    {
                        basketItem = new BasketItemViewModel()
                        {
                            BookId = bookId,
                            Count = 1
                        };

                        basketItems.Add(basketItem);
                    }
                }
                else
                {
                    basketItem = new BasketItemViewModel()
                    {
                        BookId = bookId,
                        Count = 1
                    };

                    basketItems.Add(basketItem);
                }
            }
            else
            {
                userBasketItem = await _context.BasketItems.FirstOrDefaultAsync(bi => bi.AppUserId == appUser.Id && bi.BookId == bookId);

                if(userBasketItem is not null && !userBasketItem.IsDeleted) 
                { 
                    userBasketItem.Count++;
                }
                else
                {
                    userBasketItem = new BasketItem()
                    {
                        BookId = bookId,
                        Count = 1,
                        AppUserId = appUser.Id,
                        IsDeleted = false,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                    };

                    await _context.BasketItems.AddAsync(userBasketItem);
                }
                await _context.SaveChangesAsync();
            }

            basketItemsStr = JsonConvert.SerializeObject(basketItems);

            HttpContext.Response.Cookies.Append("BasketItems", basketItemsStr);

            return Ok(); //200
        }

        public async Task<IActionResult> GetBasketItems()
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<BasketItem> userBasketItems = new List<BasketItem>();
            AppUser user = null;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            }

            if (user is not null)
            {
                userBasketItems = await _context.BasketItems.Where(bi => bi.AppUserId == user.Id && bi.IsDeleted == false).ToListAsync();

                //foreach (var userBasketItem in userBasketItems)
                //{
                //    BasketItemViewModel basketItemViewModel = new BasketItemViewModel()
                //    {
                //        BookId = userBasketItem.BookId,
                //        Count = userBasketItem.Count,
                //    };
                //    basketItems.Add(basketItemViewModel);
                //}

                basketItems = userBasketItems.Select(ubi => new BasketItemViewModel() { BookId = ubi.BookId, Count = ubi.Count }).ToList();
            }
            else
            {
                var basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

                if (basketItemsStr is not null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);
                }
            }
            

            return Ok(basketItems);

        }

        public IActionResult Checkout()
        {
            return View();
        }

        //public IActionResult SetSession(int id)
        //{
        //    HttpContext.Session.SetString("UserName", id.ToString());

        //    return Content("Added to session");
        //}

        //public IActionResult GetSession()
        //{
        //    string? name =  HttpContext.Session.GetString("UserName");

        //    return Content(name);
        //}

        //public IActionResult RemoveSession()
        //{
        //    HttpContext.Session.Remove("UserName");
        //    return Content("Gorbagor oldu");

        //}

        //public IActionResult SetCookie(int id)
        //{
        //    List<int> ids = new List<int>();

        //    var idsStr = HttpContext.Request.Cookies["Ids"];

        //    if(idsStr is not null)
        //    {
        //        ids = JsonConvert.DeserializeObject<List<int>>(idsStr);
        //        ids.Add(id);
        //    }
        //    else
        //    {
        //        ids.Add(id);
        //    }

        //    idsStr = JsonConvert.SerializeObject(ids);

        //    HttpContext.Response.Cookies.Append("Ids", idsStr);

        //    return Content("Added to Cookie");
        //}

        //public IActionResult GetCookie()
        //{
        //    List<int> ids = new List<int>();
        //    var idsStr = HttpContext.Request.Cookies["Ids"];

        //    if(idsStr is not null) 
        //    {
        //        ids = JsonConvert.DeserializeObject<List<int>>(idsStr);
        //    }

        //    return Ok(ids);
        //}


    }
}
