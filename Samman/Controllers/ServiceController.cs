using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Samman.DataBase;
using Samman.Models;
using System.Net.Mime;

namespace Samman.Controllers
{
    public class ServiceController : Controller
    {

        public IActionResult Archiv()
        {
            return View();
        }

    }
}
