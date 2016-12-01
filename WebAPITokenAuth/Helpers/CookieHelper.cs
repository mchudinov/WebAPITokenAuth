using System.Web;

namespace Gui.Helpers
{
    public class CookieHelper
    {
        public static void SaveSessionCookie(string name, string value, HttpContext context)
        {
            var cookie = new HttpCookie(name, value);
            context.Request.Headers.Add("Cookie", name + "=" + cookie.Value);
            context.Response.Cookies.Add(cookie);
        }
    }
}
