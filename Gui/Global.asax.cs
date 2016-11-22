using System;
using System.Diagnostics;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Xml;
using Newtonsoft.Json;

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
            //SaveTokenInSession(bootstrapContext);
            //SaveTokenInCookie(bootstrapContext);
            Debug.WriteLine("Session_Start. Identity name:" + identity.Name + " IsAuthenticated:" + identity.IsAuthenticated);
        }

        private void SaveTokenInCookie(BootstrapContext bootstrapContext)
        {
            string token = GetTokenAsXml(bootstrapContext);
            string tokenBase64 = Base64Encode(token);
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("token")
            {
                Value = tokenBase64,
                Expires = DateTime.Now.AddHours(1)
            });
        }

        private void SaveTokenInSession(BootstrapContext bootstrapContext)
        {
            string token = GetTokenAsXml(bootstrapContext);
            string tokenBase64 = Base64Encode(token);
            Session["token"] = tokenBase64;
        }

        private static string GetTokenAsXml(BootstrapContext bootstrapContext)
        {
            if (!string.IsNullOrEmpty(bootstrapContext.Token))
                return bootstrapContext.Token;

             Saml2SecurityToken securityToken = bootstrapContext.SecurityToken as Saml2SecurityToken;
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

        //public static TokenResponse ConvertSamlToJwt(SecurityToken securityToken, string scope, X509Certificate2 signingCertificate)
        //{
        //    var subject = ValidateSamlToken(securityToken);

        //    var descriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = subject,
        //        AppliesToAddress = scope,
        //        SigningCredentials = new X509SigningCredentials(signingCertificate),
        //        TokenIssuerName = "https://panav.mycomp.com",
        //        Lifetime = new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10080))
        //    };


        //    var jwtHandler = new JwtSecurityTokenHandler();
        //    var jwt = jwtHandler.CreateToken(descriptor);

        //    return new TokenResponse
        //    {
        //        AccessToken = jwtHandler.WriteToken(jwt),
        //        ExpiresIn = 10080
        //    };
        //}

        //public static ClaimsIdentity ValidateSamlToken(SecurityToken securityToken)
        //{
        //    var registry = new ConfigurationBasedIssuerNameRegistry();
        //    registry.AddTrustedIssuer("115E38C827ACE56638E32935604C52AF83DFA544", "https://mikael.accesscontrol.windows.net/");

        //    var configuration = new SecurityTokenHandlerConfiguration
        //    {
        //        AudienceRestriction = { AudienceMode = AudienceUriMode.Never },
        //        CertificateValidationMode = X509CertificateValidationMode.None,
        //        RevocationMode = X509RevocationMode.NoCheck,
        //        CertificateValidator = X509CertificateValidator.None,
        //        IssuerNameRegistry = registry
        //    };

        //    var handler = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection(configuration);
        //    var identity = handler.ValidateToken(securityToken).First();
        //    return identity;
        //}

    }

    public class TokenResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }


        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }


        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }


        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}
