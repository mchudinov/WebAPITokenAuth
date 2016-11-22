using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common
{
    public class CookieHelper
    {
        public static void SaveInSessionCookie(string name, string value, HttpContext context)
        {
            var chunks = SplitString(value, 2000);
            for (int i = 0; i < chunks.Count(); i++)
            {
                SaveSessionCookieInternal(name + i, chunks.ElementAt(i), context);
            }
        }

        private static IEnumerable<string> SplitString(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        private static void SaveSessionCookieInternal(string name, string value, HttpContext context)
        {
            HttpCookie cookie = new HttpCookie(name, value);
            context.Request.Headers.Add("Cookie", name + "=" + cookie.Value);
            context.Response.Cookies.Add(cookie);
        }
    }
}
