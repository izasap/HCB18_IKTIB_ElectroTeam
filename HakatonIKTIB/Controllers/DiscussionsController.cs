using HakatonIKTIB.Classes;
using HakatonIKTIB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HakatonIKTIB.Controllers
{
    public class DiscussionsController : Controller
    {
        // GET: DiscussionPageController
        public IActionResult Index(ulong discussionId, ulong userId)
        {
            Discussion? discus = DataBase.GetDiscussionById(discussionId);
            UserModel model = new();
            if (userId == 0)
            {
                model = new(discus);
                return View(model);
            }

            User? user = DataBase.GetUserById(userId);
            model = new(discus, user);
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateDiscussion(ulong userId)
        {
            User? user = DataBase.GetUserById(userId);
            UserModel model = new(user);
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateDiscussion(string applicantId, string topic, string text)
        {
            ulong userId = ulong.Parse(applicantId);
            DataBase.CreateDiscussion(userId, topic, text);
            return RedirectToAction("Index", "Home", new { userId } );
        }

        [HttpGet]
        public IActionResult AddAnswer(ulong discussionId, ulong userId) => RedirectToAction("Index", "Discussions", new { discussionId, userId });

        [HttpPost]
        public IActionResult AddAnswer(string answerDiscussionId, string answerUserId, string answerText)
        {
            ulong discussionId = ulong.Parse(answerDiscussionId);
            ulong userId = ulong.Parse(answerUserId);
            DataBase.AddAnswer(discussionId, userId, answerText);
            UserModel model = new(DataBase.GetDiscussionById(discussionId), DataBase.GetUserById(userId));
            return RedirectToAction("Index", "Discussions", new { discussionId, userId });
        }
    }
}
