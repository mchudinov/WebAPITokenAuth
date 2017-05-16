using System;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Xml;
using Gui.Models;

namespace Gui.Helpers
{
    public class SecurityTokenHelper
    {
        private static bool IsTokenExpired(SecurityToken token)
        {
            return token == null || (DateTime.UtcNow < token.ValidFrom || DateTime.UtcNow > token.ValidTo);
        }

        private static void SaveObjectInCache(object obj, string key)
        {
            int expirationMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["KeyCacheExpirationMinutes"]);
            HttpRuntime.Cache.Add(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, expirationMinutes, 0), System.Web.Caching.CacheItemPriority.Normal, null);
        }

        private static SecurityToken GetTokenFromCache(string key)
        {
            return HttpRuntime.Cache[key.Trim()] as SecurityToken;
        }

        public static string GetKey()
        {
            return Guid.NewGuid().ToString();
        }

        private static string GetKeyFromRequest(HttpRequestMessage request, out ErrorCode? error)
        {
            var authHeader = request?.Headers?.Authorization;
            if (string.IsNullOrWhiteSpace(authHeader?.Parameter))
            {
                error = ErrorCode.SECURITY_TOKEN_MISSING;
                return string.Empty;
            }

            if (authHeader.Scheme != "Bearer")
            {
                error = ErrorCode.SECURITY_TOKEN_INVALID;
                return string.Empty;
            }

            error = null;
            return authHeader.Parameter;
        }

        public static SecurityToken GetTokenFromRequest(HttpRequestMessage request, out ErrorCode? error)
        {
            SecurityToken token = null;
            string key = GetKeyFromRequest(request, out error);

            if (null != error)
                return null;

            if (string.IsNullOrEmpty(key))
            {
                error = ErrorCode.SECURITY_TOKEN_INVALID;
                return null;
            }

            try
            {
                token = GetTokenFromCache(key);
            }
            catch
            {
                error = ErrorCode.SECURITY_TOKEN_VALIDATION_ERROR;
                return null;
            }

            if (null == token)
            {
                error = ErrorCode.SECURITY_TOKEN_INVALID;
                return null;
            }

            if (IsTokenExpired(token))
            {
                error = ErrorCode.SECURITY_TOKEN_EXPIRED;
                return null;
            }

            return token;
        }

        private static SecurityToken GetTokenFromXml(string tokenXml)
        {
            if (string.IsNullOrEmpty(tokenXml))
                return null;

            var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
            SecurityToken token;
            try
            {
                using (StringReader stringReader = new StringReader(tokenXml))
                using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
                {
                    token = (SecurityToken)handlers.ReadToken(xmlReader);
                }
            }
            catch
            {
                return null;
            }
            return token;
        }

        public static SecurityToken GetTokenFromBootstrapContext(BootstrapContext bootstrapContext)
        {
            if (null != bootstrapContext?.SecurityToken)
                return bootstrapContext.SecurityToken as SecurityToken;

            if (!string.IsNullOrWhiteSpace(bootstrapContext?.Token))
                return GetTokenFromXml(bootstrapContext.Token);

            return null;
        }

        public static void StoreSecurityToken(SecurityToken token)
        {
            if (null == token)
                return;

            string key = SecurityTokenHelper.GetKey();
            SecurityTokenHelper.SaveObjectInCache(token, key);
            CookieHelper.SaveSessionCookie("key", key, HttpContext.Current);
            Debug.WriteLine($"SecurityToken saved in cache. SecurityTokenID: {token.Id}, key: {key}");
        }
    }
}
