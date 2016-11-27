using System.Web.Mvc;

namespace Gui.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.ClaimsIdentity = System.Web.HttpContext.Current.User.Identity;
            return View();
        }
    }
}