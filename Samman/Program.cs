using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Samman.DataBase;

namespace Samman
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var dbContext = new AccountDbContext())
            {
                dbContext.Database.EnsureCreated();
            }
            using (var dbContext = new DocFileDbContext())
            {
                dbContext.Database.EnsureCreated();
            }
            using (var dbContext = new DocNamesDbContext())
            {
                dbContext.Database.EnsureCreated();
            }
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Program>();
                })
                .Build();

            host.Run();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "YourSessionCookieName";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Время жизни аутентификации
            options.LoginPath = "/Autorisation/Index"; // Путь к странице входа
            options.AccessDeniedPath = "/Account/LoginError"; // Путь к странице запрета доступа
            options.SlidingExpiration = true;
        });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Autorisation}/{action=Logout}/{id?}");
            });
        }
    }
}
