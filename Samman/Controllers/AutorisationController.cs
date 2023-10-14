using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samman.DataBase;
using Samman.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Samman.Controllers
{
    public class AutorisationController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Undex()
        {
            return View();
        }

        public async Task<IActionResult> Registration(User model)
        {
            if (ModelState.IsValid)
            {
                // Проверяем, существует ли уже пользователь с таким именем
                using var db = new AccountDbContext();
                var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    return RedirectToAction("LoginError", "Home");
                }

                // Проверяем длину имени пользователя
                if (model.Username.Length < 6)
                {
                    ModelState.AddModelError("Username", "Имя пользователя должно содержать не менее 6 символов.");
                    return RedirectToAction("LoginError", "Home");
                }

                // Хэширование пароля
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Хэширование секретного ответа

                User user = new User
                {
                    Username = model.Username,
                    Password = hashedPassword,
                    Mail = model.Mail,
                    Gender = model.Gender
                };

                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();

                // Остальной код

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString("Username", model.Username);
                HttpContext.Session.SetString("Gender", user.Gender);
                HttpContext.Session.SetInt32("IdUser", user.Id);

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("LoginError", "Home");
        }

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new AccountDbContext();

                // Найти пользователя по имени
                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null)
                {
                    // Проверить хэш пароля
                    if (BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                    {
                        // Успешная аутентификация - установить данные пользователя в сессию
                        HttpContext.Session.SetString("Username", user.Username);
                        HttpContext.Session.SetInt32("IdUser", user.Id);
                        HttpContext.Session.SetString("Gender", user.Gender);

                        // Остальной код, если есть

                        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        // Остальной код, если есть

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Неверные имя пользователя или пароль");
                return RedirectToAction("LoginError", "Home");
            }

            return RedirectToAction("LoginError", "Home");
        }
    

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
