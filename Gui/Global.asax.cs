using System;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Gui
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(Gui.WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var principal = (ClaimsPrincipal)System.Web.HttpContext.Current.User;
            var identity = principal.Identity;
            var bootstrapContext = (BootstrapContext)principal.Identities.First().BootstrapContext;
            Debug.WriteLine("Session_Start. Identity name:" + identity.Name + " IsAuthenticated:" + identity.IsAuthenticated);
        }
    }
}
