using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokMVC.Data;
using PustokMVC.Models;
using PustokMVC.ViewModels;

namespace PustokMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly PustokDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, 
                                 PustokDbContext context,
                                 SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (!ModelState.IsValid) return View();
            AppUser member = new AppUser()
            {
                Fullname = model.Fullname,
                UserName = model.Username,
                Email = model.Email,
            };
            if(_context.Users.Any(x=>x.NormalizedUserName == model.Username.ToUpper()))
            {
                ModelState.AddModelError("UserName", "Username is already exist!");
                return View();
            }
            if (_context.Users.Any(x => x.NormalizedEmail == model.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email is already exist!");
                return View();
            }

            var result = await _userManager.CreateAsync(member,model.Password);

            if(!result.Succeeded) 
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                    return View();
                }
            }

            var roleResult = await _userManager.AddToRoleAsync(member, "Member");

            return RedirectToAction("login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel userLoginViewModel)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = null;

            user = await _userManager.FindByNameAsync(userLoginViewModel.UsernameOrEmail);

            if(user is null) 
            {
                user = await _userManager.FindByEmailAsync(userLoginViewModel.UsernameOrEmail);

                if( user is null )
                {
                    ModelState.AddModelError("", "Invalid credentials");
                    return View();
                }
            }

            var result = await _signInManager.PasswordSignInAsync(user, userLoginViewModel.Password, false, false);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View();  
            }

            return RedirectToAction(nameof(Index), "home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login), "account");
        }
    }
}
