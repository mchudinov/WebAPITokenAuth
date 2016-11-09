using System.Web.Mvc;

namespace Gui.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Your claims page";
            ViewBag.ClaimsIdentity = System.Web.HttpContext.Current.User.Identity;

            //string authHeader = ar.CreateAuthorizationHeader();
            //HttpClient client = new HttpClient();
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44353/api/Default");
            //request.Headers.TryAddWithoutValidation("Authorization", authHeader);
            //HttpResponseMessage response = await client.SendAsync(request);
            //string responseString = await response.Content.ReadAsStringAsync();

            return View();
        }
    }
}