using System.Diagnostics;
using HakatonIKTIB.Models;
using Microsoft.AspNetCore.Mvc;
using HakatonIKTIB.Classes;

namespace HakatonIKTIB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index(ulong userId)
        {
            if (userId == 0)
            {
                return View(new UserModel());
            }
            else
            {
                return View(new UserModel(DataBase.GetUserById(userId)));
            }
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
