﻿using System;
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
using Common;

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
            string tokenXml = SecurityTokenHelper.GetSaml2TokenAsXml(bootstrapContext);
            string tokenBase64 = Base64.Encode(tokenXml);
            //HttpContext.Current.Session["token"] = tokenBase64;   //TDOD Save is session
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
    }
}