using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Gui
{
    /// <summary>
    /// Capture WSFederationAuthenticationModule events
    /// https://msdn.microsoft.com/en-us/library/system.identitymodel.services.wsfederationauthenticationmodule
    /// </summary>
    sealed class CustomWSFederationAuthenticationModule : WSFederationAuthenticationModule
    {
        public CustomWSFederationAuthenticationModule()
        {
            base.AuthorizationFailed += CustomAuthenticationModule_AuthorizationFailed;
            base.RedirectingToIdentityProvider += CustomAuthenticationModule_RedirectingToIdentityProvider;
            base.SecurityTokenReceived += CustomAuthenticationModule_SecurityTokenReceived;
            base.SecurityTokenValidated += CustomAuthenticationModule_SecurityTokenValidated;
            base.SessionSecurityTokenCreated += CustomAuthenticationModule_SessionSecurityTokenCreated;
            base.SignedIn += CustomAuthenticationModule_SignedIn;
            base.SignedOut += CustomAuthenticationModule_SignedOut;
            base.SignInError += CustomAuthenticationModule_SignInError;
            base.SigningOut += CustomAuthenticationModule_SigningOut;
            base.SignOutError += CustomAuthenticationModule_SignOutError;
        }

        private void CustomAuthenticationModule_SignOutError(object sender, ErrorEventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule)sender;
            Debug.WriteLine("SignOutError. Message: " + e.Exception.Message);
        }

        private void CustomAuthenticationModule_SigningOut(object sender, SigningOutEventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule)sender;
            Debug.WriteLine("SigningOut");
        }

        private void CustomAuthenticationModule_SignInError(object sender, ErrorEventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule)sender;
            Debug.WriteLine("SignInError. Message: " + e.Exception.Message);
        }

        private void CustomAuthenticationModule_SignedOut(object sender, EventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule)sender;
            Debug.WriteLine("SignedOut");
        }

        private void CustomAuthenticationModule_SignedIn(object sender, EventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule)sender;
            Debug.WriteLine("SignedIn");

            var principal = (ClaimsPrincipal)System.Web.HttpContext.Current.User;
            var bootstrapContext = (BootstrapContext)principal.Identities.First().BootstrapContext;
            var claims = principal.Claims;
            var token = bootstrapContext.SecurityToken;
            SaveTokenInCookie(bootstrapContext);
            //SaveTokenInSession(bootstrapContext);
        }

        private void CustomAuthenticationModule_SessionSecurityTokenCreated(object sender, SessionSecurityTokenCreatedEventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule)sender;
            var token = (System.IdentityModel.Tokens.SessionSecurityToken) e.SessionToken;
            Debug.WriteLine("SessionSecurityTokenCreated. TokenId:" + token.Id + " KeyExpirationTime:" + token.KeyExpirationTime);
        }

        private void CustomAuthenticationModule_SecurityTokenValidated(object sender, SecurityTokenValidatedEventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule)sender;
            Debug.WriteLine("SecurityTokenValidated");
        }

        private void CustomAuthenticationModule_RedirectingToIdentityProvider(object sender, RedirectingToIdentityProviderEventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule)sender;
            Debug.WriteLine("RedirectingToIdentityProvider. SignInRequestMessage:" + e.SignInRequestMessage);
        }

        private void CustomAuthenticationModule_AuthorizationFailed(object sender, AuthorizationFailedEventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule) sender;
            Debug.WriteLine("AuthorizationFailed. RedirectToIdentityProvider:" + e.RedirectToIdentityProvider);
        }

        private void CustomAuthenticationModule_SecurityTokenReceived(object sender, SecurityTokenReceivedEventArgs e)
        {
            var auth = (CustomWSFederationAuthenticationModule)sender;
            Debug.WriteLine("SecurityTokenReceived. SecurityToken:" + e.SecurityToken + " SignInContext:" + e.SignInContext);
        }

        protected override void OnRedirectingToIdentityProvider(RedirectingToIdentityProviderEventArgs e)
        {
            if ((new HttpRequestWrapper(HttpContext.Current.Request)).IsAjaxRequest())
                e.Cancel = true;
            base.OnRedirectingToIdentityProvider(e);
        }

        private void SaveTokenInCookie(BootstrapContext bootstrapContext)
        {
            string token = GetTokenAsXml(bootstrapContext);
            string tokenBase64 = Base64Encode(token);

            var chunks = SplitString(tokenBase64, 2000);
            for (int i = 0; i < chunks.Count(); i++)
            {
                SaveCookieInternal("token"+i, chunks.ElementAt(i));
            }
        }

        static IEnumerable<string> SplitString(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        private void SaveCookieInternal(string name, string value)
        {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = new HttpCookie(name, value)
            {
                Expires = DateTime.Now.AddYears(1)
            };
            context.Request.Headers.Add("Cookie", name + "=" + cookie.Value);
            context.Response.Cookies.Add(cookie);
        }

        private void SaveTokenInSession(BootstrapContext bootstrapContext)
        {
            string token = GetTokenAsXml(bootstrapContext);
            string tokenBase64 = Base64Encode(token);
            HttpContext.Current.Session["token"] = tokenBase64;
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
    }
}