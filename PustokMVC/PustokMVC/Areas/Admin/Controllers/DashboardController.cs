using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokMVC.Models;

namespace PustokMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DashboardController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser admin = new AppUser()
        //    {
        //        Fullname = "Super Admin",
        //        UserName = "SuperAdmin",
        //        Email = "superadmin@gmail.com",
        //    };
        //    string password = "SuperAdmin123@";
        //    var result =  await _userManager.CreateAsync(admin, password);

        //    return  Ok(result);
        //}

        //public async Task<IActionResult> CreateRole()
        //{
        //    IdentityRole role1 = new IdentityRole("SuperAdmin");
        //    IdentityRole role2 = new IdentityRole("Admin");
        //    IdentityRole role3 = new IdentityRole("Member");

        //    await _roleManager.CreateAsync(role1);
        //    await _roleManager.CreateAsync(role2);
        //    await _roleManager.CreateAsync(role3);

        //    return Ok();
        //}

        //public async Task<IActionResult> AddRole()
        //{
        //    AppUser admin = await _userManager.FindByNameAsync("SuperAdmin");


        //    var result = await _userManager.AddToRoleAsync(admin, "SuperAdmin");

        //    return Ok(result);
        //}
    }
}
