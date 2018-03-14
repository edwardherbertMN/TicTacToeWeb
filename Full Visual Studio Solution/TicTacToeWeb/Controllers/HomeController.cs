using Microsoft.AspNetCore.Mvc;

namespace TicTacToeWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
