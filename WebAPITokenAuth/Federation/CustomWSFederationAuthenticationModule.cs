using System;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Web;
using System.Web.Mvc;
using Gui.Helpers;

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
            SecurityTokenHelper.StoreSecurityToken(e.SecurityToken);
            Debug.WriteLine($"SecurityTokenReceived. SecurityToken ID: {e.SecurityToken.Id}");
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