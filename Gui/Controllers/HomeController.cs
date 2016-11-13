﻿using System.IO;
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
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        ViewBag.Response = reader.ReadToEnd();
                        ViewBag.ResponseStatus = response.StatusDescription;
                        ViewBag.ResponseCode = (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                using (HttpWebResponse response = (HttpWebResponse)ex.Response)
                {
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        ViewBag.Response = reader.ReadToEnd();
                        ViewBag.ResponseStatus = response.StatusDescription;
                        ViewBag.ResponseCode = (int)response.StatusCode;
                    }
                }
            }
            
            return View();
        }
    }
}