﻿using System;
using System.IdentityModel.Services;
using System.Web.Mvc;

namespace Gui.Controllers
{
    public class AccountController : Controller
    {
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                FederatedAuthentication.WSFederationAuthenticationModule.SignIn(String.Empty);
            }
        }

        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);
            FederatedAuthentication.WSFederationAuthenticationModule.SignOut(callbackUrl);
            WSFederationAuthenticationModule.FederatedSignOut(new Uri(FederatedAuthentication.WSFederationAuthenticationModule.Issuer), new Uri(callbackUrl));
        }

        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}