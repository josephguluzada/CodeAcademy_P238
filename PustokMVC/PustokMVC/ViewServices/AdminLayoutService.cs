using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokMVC.Models;

namespace PustokMVC.ViewServices
{
    public class AdminLayoutService 
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminLayoutService(UserManager<AppUser> userManager,
                      IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppUser> GetUser()
        {
            AppUser? appUser = null;
            string name =  _httpContextAccessor.HttpContext.User.Identity.Name;

            if(name is not null)
                appUser = await _userManager.FindByNameAsync(name);
            return appUser;
        }
    }
}
