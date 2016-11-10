using System.IO;
using System.Net;
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

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:63046/api/Default");
            request.Headers["X-My-Custom-Header"] = Session["token"].ToString();
            request.Headers.Add(HttpRequestHeader.Authorization, Session["token"].ToString());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            ViewBag.Response = reader.ReadToEnd();
            
            return View();
        }
    }
}