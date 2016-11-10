using System;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Routing;
using System.Xml;

namespace Gui
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var principal = (ClaimsPrincipal)System.Web.HttpContext.Current.User;
            var identity = principal.Identity;
            var bootstrapContext = (BootstrapContext)principal.Identities.First().BootstrapContext;
            SaveTokenInSession(bootstrapContext.SecurityToken as Saml2SecurityToken);
            Debug.WriteLine("Session_Start. Identity name:" + identity.Name + " IsAuthenticated:" + identity.IsAuthenticated);
        }

        private void SaveTokenInSession(Saml2SecurityToken securityToken)
        {
            string token = GetTokenAsXml(securityToken);
            string tokenBase64 = Base64Encode(token);
            Session["token"] = tokenBase64;
        }

        private static string GetTokenAsXml(Saml2SecurityToken securityToken)
        {
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder))
            {
                new Saml2SecurityTokenHandler(new SamlSecurityTokenRequirement()).WriteToken(writer, securityToken);
            }
            return builder.ToString();
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
