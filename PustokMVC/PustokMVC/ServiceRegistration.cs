using PustokMVC.Business.Implementations;
using PustokMVC.Business.Interfaces;
using PustokMVC.ViewServices;

namespace PustokMVC
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<AdminLayoutService>();
        }
    }
}
