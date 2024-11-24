using HakatonIKTIB.Models;
using HakatonIKTIB.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace HakatonIKTIB.Controllers
{
    public class UserPageController : Controller
    {
        // GET: UserPage
        public IActionResult Index(string login, string password)
        {
            UserModel model = new(DataBase.Login(login, password));
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Regiser() => View();

        [HttpPost]
        public async Task<IActionResult> Regiser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ulong userId = DataBase.Register(model.Login, model.Password, model.Name, model.LastName, model.SurName, model.Course, model.Specialization, model.University , out string s).Id;
                return RedirectToAction(actionName: "Index", controllerName: "Home", new { userId });
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                ulong userId = DataBase.Login(model.Login, model.Password).Id;
                return RedirectToAction(actionName: "Index", controllerName: "Home", new { userId });
            }

            return View();
        }
    }
}
