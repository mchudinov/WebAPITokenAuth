using System;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Web;
using System.Web.Mvc;
using Gui.Helpers;
using Lindorff.Access.Web.Helpers;

namespace Gui.Federation
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/system.identitymodel.services.wsfederationauthenticationmodule
    /// </summary>
    sealed class CustomWSFederationAuthenticationModule : WSFederationAuthenticationModule
    {
        public CustomWSFederationAuthenticationModule()
        {
            base.SecurityTokenReceived += CustomAuthenticationModule_SecurityTokenReceived;
        }

        private void CustomAuthenticationModule_SecurityTokenReceived(object sender, SecurityTokenReceivedEventArgs e)
        {
            string key = SecurityTokenHelper.GetKey();
            SecurityTokenHelper.SaveTokenInCache(e.SecurityToken, key);
            CookieHelper.SaveSessionCookie("token", key, HttpContext.Current);
            Debug.WriteLine($"SecurityTokenReceived. SecurityToken ID: {e.SecurityToken.Id}, key: {key}");
        }

        protected override void OnRedirectingToIdentityProvider(RedirectingToIdentityProviderEventArgs e)
        {
            //"X-Requested-With", "XMLHttpRequest"
            if ((new HttpRequestWrapper(HttpContext.Current.Request)).IsAjaxRequest())
                e.Cancel = true;
            base.OnRedirectingToIdentityProvider(e);
        }
    }
}